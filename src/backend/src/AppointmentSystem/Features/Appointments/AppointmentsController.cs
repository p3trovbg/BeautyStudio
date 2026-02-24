using AppointmentSystem.Features.Appointments.DTOs;
using AppointmentSystem.Features.Appointments.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSystem.Features.Appointments;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _service;

    public AppointmentsController(IAppointmentService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? ownerId, 
        [FromQuery] Guid? customerId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10, 
        CancellationToken ct = default)
    {
        var result = await _service.GetAllAsync(ownerId, customerId, page, pageSize, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        if (result.IsFailure && result.ErrorCode == "NOT_FOUND") return NotFound();
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpGet("check-overlap")]
    public async Task<IActionResult> CheckOverlap([FromQuery] Guid ownerId, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime, [FromQuery] Guid? excludeAppointmentId, CancellationToken ct)
    {
        var result = await _service.CheckOverlapAsync(ownerId, startTime, endTime, excludeAppointmentId, ct);
        if (result.IsFailure) return BadRequest(new { Error = result.Errors.First() });
        return Ok(new { HasOverlap = result.Value });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAppointmentDto dto, CancellationToken ct)
    {
        var result = await _service.CreateAsync(dto, ct);
        if (result.IsFailure && result.ErrorCode == "APPOINTMENT_OVERLAP")
            return Conflict(new { Message = result.Errors.First(), ErrorCode = result.ErrorCode });

        return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value) : BadRequest(result.Errors);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateAppointmentDto dto, CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, dto, ct);
        if (result.IsFailure && result.ErrorCode == "NOT_FOUND") return NotFound();
        if (result.IsFailure && result.ErrorCode == "APPOINTMENT_OVERLAP")
            return Conflict(new { Message = result.Errors.First(), ErrorCode = result.ErrorCode });
            
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        var result = await _service.CancelAsync(id, ct);
        if (result.IsFailure && result.ErrorCode == "NOT_FOUND") return NotFound();
        return result.IsSuccess ? NoContent() : BadRequest(result.Errors);
    }
}
