using AppointmentSystem.Common.Application;
using AppointmentSystem.Features.Appointments.DTOs;

namespace AppointmentSystem.Features.Appointments.Services;

/// <summary>Service for managing appointments.</summary>
public interface IAppointmentService
{
    Task<Result<PagedResult<AppointmentDto>>> GetAllAsync(Guid? ownerId, Guid? customerId, int page, int pageSize, CancellationToken ct);
    Task<Result<AppointmentDto>> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Result<AppointmentDto>> CreateAsync(CreateAppointmentDto dto, CancellationToken ct);
    Task<Result<AppointmentDto>> UpdateAsync(Guid id, UpdateAppointmentDto dto, CancellationToken ct);
    Task<Result> CancelAsync(Guid id, CancellationToken ct);
    Task<Result<bool>> CheckOverlapAsync(Guid ownerId, DateTime start, DateTime end, Guid? excludeAppointmentId, CancellationToken ct);
}
