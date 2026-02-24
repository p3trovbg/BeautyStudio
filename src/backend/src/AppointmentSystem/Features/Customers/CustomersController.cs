using AppointmentSystem.Features.Customers.DTOs;
using AppointmentSystem.Features.Customers.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSystem.Features.Customers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _service;

    public CustomersController(ICustomerService service) => _service = service;

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
    public async Task<IActionResult> Create(CreateCustomerDto dto, CancellationToken ct)
    {
        var result = await _service.CreateAsync(dto, ct);
        return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value) : BadRequest(result.Errors);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateCustomerDto dto, CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, dto, ct);
        if (result.IsFailure && result.ErrorCode == "NOT_FOUND") return NotFound();
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _service.DeleteAsync(id, ct);
        if (result.IsFailure && result.ErrorCode == "NOT_FOUND") return NotFound();
        return result.IsSuccess ? NoContent() : BadRequest(result.Errors);
    }
}
