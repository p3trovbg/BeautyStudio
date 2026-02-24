using AppointmentSystem.Common.Application;
using AppointmentSystem.Features.Customers.DTOs;

namespace AppointmentSystem.Features.Customers.Services;

/// <summary>Service for managing customers.</summary>
public interface ICustomerService
{
    Task<Result<PagedResult<CustomerDto>>> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<Result<CustomerDto>> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Result<CustomerDto>> CreateAsync(CreateCustomerDto dto, CancellationToken ct);
    Task<Result<CustomerDto>> UpdateAsync(Guid id, UpdateCustomerDto dto, CancellationToken ct);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct);
}
