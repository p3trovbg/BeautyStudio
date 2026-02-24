using AppointmentSystem.Features.Owners.DTOs;
using AppointmentSystem.Features.Owners.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSystem.Features.Owners;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OwnersController : ControllerBase
{
    private readonly IOwnerService _service;

    public OwnersController(IOwnerService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var result = await _service.GetAllAsync(page, pageSize, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        if (result.IsFailure && result.ErrorCode == "NOT_FOUND") return NotFound();
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpPost]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> Create(CreateOwnerDto dto, CancellationToken ct)
    {
        var result = await _service.CreateAsync(dto, ct);
        return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value) : BadRequest(result.Errors);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> Update(Guid id, UpdateOwnerDto dto, CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, dto, ct);
        if (result.IsFailure && result.ErrorCode == "NOT_FOUND") return NotFound();
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Owner")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(id, ct);
        if (result.IsFailure && result.ErrorCode == "NOT_FOUND") return NotFound();
        return result.IsSuccess ? NoContent() : BadRequest(result.Errors);
    }
}
