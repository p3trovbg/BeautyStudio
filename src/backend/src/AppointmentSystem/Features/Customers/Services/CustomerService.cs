using AppointmentSystem.Common.Domain;
using AppointmentSystem.Common.Application;
using AppointmentSystem.Common.Persistence;
using AppointmentSystem.Features.Customers.Domain;
using AppointmentSystem.Features.Customers.DTOs;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace AppointmentSystem.Features.Customers.Services;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateCustomerDto> _createValidator;
    private readonly IValidator<UpdateCustomerDto> _updateValidator;

    public CustomerService(AppDbContext context, IMapper mapper, IValidator<CreateCustomerDto> createValidator, IValidator<UpdateCustomerDto> updateValidator)
    {
        _context = context;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<PagedResult<CustomerDto>>> GetAllAsync(int page, int pageSize, CancellationToken ct)
    {
        var query = _context.Customers.AsNoTracking();
        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return Result<PagedResult<CustomerDto>>.Success(new PagedResult<CustomerDto>(_mapper.Map<List<CustomerDto>>(items), page, pageSize, total));
    }

    public async Task<Result<CustomerDto>> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (customer is null) return Result<CustomerDto>.Failure($"Customer {id} not found.", "NOT_FOUND");
        return Result<CustomerDto>.Success(_mapper.Map<CustomerDto>(customer));
    }

    public async Task<Result<CustomerDto>> CreateAsync(CreateCustomerDto dto, CancellationToken ct)
    {
        var validation = await _createValidator.ValidateAsync(dto, ct);
        if (!validation.IsValid) return Result<CustomerDto>.Failure(validation.Errors.Select(e => e.ErrorMessage).ToList(), "VALIDATION_ERROR");

        var email = Email.Create(dto.Email);
        if (await _context.Customers.AnyAsync(x => x.Email == email, ct))
            return Result<CustomerDto>.Failure("Email is already registered.", "DUPLICATE_EMAIL");

        var customer = Customer.Create(dto.FullName, email, dto.PhoneNumber);
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(ct);
        return Result<CustomerDto>.Success(_mapper.Map<CustomerDto>(customer));
    }

    public async Task<Result<CustomerDto>> UpdateAsync(Guid id, UpdateCustomerDto dto, CancellationToken ct)
    {
        var validation = await _updateValidator.ValidateAsync(dto, ct);
        if (!validation.IsValid) return Result<CustomerDto>.Failure(validation.Errors.Select(e => e.ErrorMessage).ToList(), "VALIDATION_ERROR");

        var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (customer is null) return Result<CustomerDto>.Failure($"Customer {id} not found.", "NOT_FOUND");

        var email = Email.Create(dto.Email);
        if (customer.Email != email && await _context.Customers.AnyAsync(x => x.Email == email, ct))
            return Result<CustomerDto>.Failure("Email is already registered.", "DUPLICATE_EMAIL");

        customer.Update(dto.FullName, email, dto.PhoneNumber);
        await _context.SaveChangesAsync(ct);
        return Result<CustomerDto>.Success(_mapper.Map<CustomerDto>(customer));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (customer is null) return Result.Failure($"Customer {id} not found.", "NOT_FOUND");

        customer.IsDeleted = true; // Soft delete
        await _context.SaveChangesAsync(ct);
        return Result.Success();
    }
}
