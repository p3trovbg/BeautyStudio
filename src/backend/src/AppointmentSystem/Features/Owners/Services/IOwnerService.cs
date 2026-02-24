using AppointmentSystem.Common.Application;
using AppointmentSystem.Features.Owners.DTOs;

namespace AppointmentSystem.Features.Owners.Services;

/// <summary>Service for managing owners.</summary>
public interface IOwnerService
{
    Task<Result<PagedResult<OwnerDto>>> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<Result<OwnerDto>> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Result<OwnerDto>> CreateAsync(CreateOwnerDto dto, CancellationToken ct);
    Task<Result<OwnerDto>> UpdateAsync(Guid id, UpdateOwnerDto dto, CancellationToken ct);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct);
}
