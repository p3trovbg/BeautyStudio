using System.Data;
using AppointmentSystem.Common.Application;
using AppointmentSystem.Common.Application.Interfaces;
using AppointmentSystem.Common.Domain;
using AppointmentSystem.Common.Persistence;
using AppointmentSystem.Features.Appointments.Domain;
using AppointmentSystem.Features.Appointments.DTOs;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppointmentSystem.Features.Appointments.Services;

public class AppointmentService : IAppointmentService
{
    private readonly AppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateAppointmentDto> _createValidator;
    private readonly IValidator<UpdateAppointmentDto> _updateValidator;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(
        AppDbContext context,
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        IMapper mapper,
        IValidator<CreateAppointmentDto> createValidator,
        IValidator<UpdateAppointmentDto> updateValidator,
        ILogger<AppointmentService> logger)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    public async Task<Result<PagedResult<AppointmentDto>>> GetAllAsync(Guid? ownerId, Guid? customerId, int page, int pageSize, CancellationToken ct)
    {
        var query = _context.Appointments.Include(x => x.Owner).Include(x => x.Customer).AsNoTracking();
        
        if (ownerId.HasValue) query = query.Where(x => x.OwnerId == ownerId.Value);
        if (customerId.HasValue) query = query.Where(x => x.CustomerId == customerId.Value);

        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return Result<PagedResult<AppointmentDto>>.Success(new PagedResult<AppointmentDto>(_mapper.Map<List<AppointmentDto>>(items), page, pageSize, total));
    }

    public async Task<Result<AppointmentDto>> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var apt = await _context.Appointments.Include(x => x.Owner).Include(x => x.Customer).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (apt is null) return Result<AppointmentDto>.Failure($"Appointment {id} not found.", "NOT_FOUND");
        return Result<AppointmentDto>.Success(_mapper.Map<AppointmentDto>(apt));
    }

    public async Task<Result<AppointmentDto>> CreateAsync(CreateAppointmentDto dto, CancellationToken ct)
    {
        var validation = await _createValidator.ValidateAsync(dto, ct);
        if (!validation.IsValid) return Result<AppointmentDto>.Failure(validation.Errors.Select(e => e.ErrorMessage).ToList(), "VALIDATION_ERROR");

        var timeRange = DateTimeRange.Create(dto.StartTime, dto.EndTime);

        await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        try
        {
            var hasOverlap = await _context.Appointments.AnyAsync(a =>
                a.OwnerId == dto.OwnerId &&
                a.Status != AppointmentStatus.Cancelled &&
                a.TimeRange.Start < timeRange.End &&
                a.TimeRange.End > timeRange.Start, ct);

            if (hasOverlap)
            {
                await _unitOfWork.RollbackTransactionAsync(ct);
                return Result<AppointmentDto>.Failure($"The time slot from {dto.StartTime:g} to {dto.EndTime:g} is not available.", "APPOINTMENT_OVERLAP");
            }

            var owner = await _context.Owners.FindAsync(new object[] { dto.OwnerId }, ct);
            var customer = await _context.Customers.FindAsync(new object[] { dto.CustomerId }, ct);

            if (owner is null) { await _unitOfWork.RollbackTransactionAsync(ct); return Result<AppointmentDto>.Failure($"Owner {dto.OwnerId} not found.", "NOT_FOUND"); }
            if (customer is null) { await _unitOfWork.RollbackTransactionAsync(ct); return Result<AppointmentDto>.Failure($"Customer {dto.CustomerId} not found.", "NOT_FOUND"); }

            var appointment = Appointment.Create(dto.OwnerId, dto.CustomerId, dto.Title, timeRange, dto.Notes);
            _context.Appointments.Add(appointment);
            await _unitOfWork.SaveChangesAsync(ct);
            await _unitOfWork.CommitTransactionAsync(ct);

            // Reload with relations for email mapping
            await _context.Entry(appointment).Reference(a => a.Owner).LoadAsync(ct);
            await _context.Entry(appointment).Reference(a => a.Customer).LoadAsync(ct);

            var mappedDto = _mapper.Map<AppointmentDto>(appointment);
            
            // Fire and forget email (in a real app, use Hangfire Enqueue here)
            _ = Task.Run(() => _emailService.SendBookingConfirmationAsync(mappedDto, CancellationToken.None));

            return Result<AppointmentDto>.Success(mappedDto);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            _logger.LogError(ex, "Error creating appointment");
            throw;
        }
    }

    public async Task<Result<AppointmentDto>> UpdateAsync(Guid id, UpdateAppointmentDto dto, CancellationToken ct)
    {
        var validation = await _updateValidator.ValidateAsync(dto, ct);
        if (!validation.IsValid) return Result<AppointmentDto>.Failure(validation.Errors.Select(e => e.ErrorMessage).ToList(), "VALIDATION_ERROR");

        var timeRange = DateTimeRange.Create(dto.StartTime, dto.EndTime);

        await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        try
        {
            var appointment = await _context.Appointments.Include(x => x.Owner).Include(x => x.Customer).FirstOrDefaultAsync(x => x.Id == id, ct);
            if (appointment is null) { await _unitOfWork.RollbackTransactionAsync(ct); return Result<AppointmentDto>.Failure($"Appointment {id} not found.", "NOT_FOUND"); }

            if (dto.Status != AppointmentStatus.Cancelled)
            {
                var hasOverlap = await _context.Appointments.AnyAsync(a =>
                    a.Id != id &&
                    a.OwnerId == appointment.OwnerId &&
                    a.Status != AppointmentStatus.Cancelled &&
                    a.TimeRange.Start < timeRange.End &&
                    a.TimeRange.End > timeRange.Start, ct);

                if (hasOverlap)
                {
                    await _unitOfWork.RollbackTransactionAsync(ct);
                    return Result<AppointmentDto>.Failure($"The time slot from {dto.StartTime:g} to {dto.EndTime:g} is not available.", "APPOINTMENT_OVERLAP");
                }
            }

            appointment.Update(dto.Title, timeRange, dto.Notes);
            appointment.Status = dto.Status;
            
            await _unitOfWork.SaveChangesAsync(ct);
            await _unitOfWork.CommitTransactionAsync(ct);

            var mappedDto = _mapper.Map<AppointmentDto>(appointment);

            if (dto.Status == AppointmentStatus.Cancelled)
            {
                 _ = Task.Run(() => _emailService.SendCancellationNoticeAsync(mappedDto, CancellationToken.None));
            }

            return Result<AppointmentDto>.Success(mappedDto);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            _logger.LogError(ex, "Error updating appointment {Id}", id);
            throw;
        }
    }

    public async Task<Result> CancelAsync(Guid id, CancellationToken ct)
    {
        var result = await UpdateAsync(id, new UpdateAppointmentDto(
            Title: "Cancelled", 
            StartTime: DateTime.UtcNow, 
            EndTime: DateTime.UtcNow.AddMinutes(1), 
            Status: AppointmentStatus.Cancelled, 
            Notes: null), ct);
            
        return result.IsSuccess ? Result.Success() : Result.Failure(result.Errors.First(), result.ErrorCode);
    }

    public async Task<Result<bool>> CheckOverlapAsync(Guid ownerId, DateTime start, DateTime end, Guid? excludeAppointmentId, CancellationToken ct)
    {
        try 
        {
            var timeRange = DateTimeRange.Create(start, end);
            var query = _context.Appointments.Where(a => 
                a.OwnerId == ownerId && 
                a.Status != AppointmentStatus.Cancelled &&
                a.TimeRange.Start < timeRange.End &&
                a.TimeRange.End > timeRange.Start);
                
            if (excludeAppointmentId.HasValue)
                query = query.Where(a => a.Id != excludeAppointmentId.Value);
                
            var hasOverlap = await query.AnyAsync(ct);
            return Result<bool>.Success(hasOverlap);
        }
        catch (DomainException ex)
        {
            return Result<bool>.Failure(ex.Message, ex.ErrorCode);
        }
    }
}
