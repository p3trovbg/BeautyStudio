using AppointmentSystem.Common.Domain;
using AppointmentSystem.Common.Application;
using AppointmentSystem.Common.Persistence;
using AppointmentSystem.Features.Owners.Domain;
using AppointmentSystem.Features.Owners.DTOs;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace AppointmentSystem.Features.Owners.Services;

public class OwnerService : IOwnerService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateOwnerDto> _createValidator;
    private readonly IValidator<UpdateOwnerDto> _updateValidator;

    public OwnerService(AppDbContext context, IMapper mapper, IValidator<CreateOwnerDto> createValidator, IValidator<UpdateOwnerDto> updateValidator)
    {
        _context = context;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<PagedResult<OwnerDto>>> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        var query = _context.Owners.AsNoTracking();
        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return Result<PagedResult<OwnerDto>>.Success(new PagedResult<OwnerDto>(_mapper.Map<List<OwnerDto>>(items), page, pageSize, total));
    }

    public async Task<Result<OwnerDto>> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var owner = await _context.Owners.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (owner is null) return Result<OwnerDto>.Failure($"Owner {id} not found.", "NOT_FOUND");
        return Result<OwnerDto>.Success(_mapper.Map<OwnerDto>(owner));
    }

    public async Task<Result<OwnerDto>> CreateAsync(CreateOwnerDto dto, CancellationToken ct)
    {
        var validation = await _createValidator.ValidateAsync(dto, ct);
        if (!validation.IsValid) return Result<OwnerDto>.Failure(validation.Errors.Select(e => e.ErrorMessage).ToList(), "VALIDATION_ERROR");

        var email = Email.Create(dto.Email);
        if (await _context.Owners.AnyAsync(x => x.Email == email, ct))
            return Result<OwnerDto>.Failure("Email is already registered.", "DUPLICATE_EMAIL");

        var owner = Owner.Create(dto.FullName, email, dto.PhoneNumber);
        _context.Owners.Add(owner);
        await _context.SaveChangesAsync(ct);
        return Result<OwnerDto>.Success(_mapper.Map<OwnerDto>(owner));
    }

    public async Task<Result<OwnerDto>> UpdateAsync(Guid id, UpdateOwnerDto dto, CancellationToken ct)
    {
        var validation = await _updateValidator.ValidateAsync(dto, ct);
        if (!validation.IsValid) return Result<OwnerDto>.Failure(validation.Errors.Select(e => e.ErrorMessage).ToList(), "VALIDATION_ERROR");

        var owner = await _context.Owners.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (owner is null) return Result<OwnerDto>.Failure($"Owner {id} not found.", "NOT_FOUND");

        var email = Email.Create(dto.Email);
        if (owner.Email != email && await _context.Owners.AnyAsync(x => x.Email == email, ct))
            return Result<OwnerDto>.Failure("Email is already registered.", "DUPLICATE_EMAIL");

        owner.Update(dto.FullName, email, dto.PhoneNumber);
        await _context.SaveChangesAsync(ct);
        return Result<OwnerDto>.Success(_mapper.Map<OwnerDto>(owner));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct)
    {
        var owner = await _context.Owners.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (owner is null) return Result.Failure($"Owner {id} not found.", "NOT_FOUND");

        owner.IsDeleted = true; // Soft delete
        await _context.SaveChangesAsync(ct);
        return Result.Success();
    }
}
