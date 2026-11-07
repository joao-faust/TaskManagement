namespace TaskManagement.src.Presentation.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.src.Shared.Dtos.Duties;
using TaskManagement.src.Domain.Services;
using TaskManagement.src.Presentation.Services;

[ApiController, Authorize, Route("duties")]
public class DutiesController(DutiesService service, JwtService jwtService) : ControllerBase
{
    private readonly DutiesService _service = service;
    private readonly JwtService _jwtService = jwtService;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SaveDutyDto dto)
    {
        var payload = _jwtService.GetPayload(User);
        var duty = await _service.CreateAsync(payload.Id, dto);

        return CreatedAtAction(nameof(FindOne), new { id = duty.Id }, null);
    }

    [HttpGet]
    public async Task<IActionResult> FindAll()
    {
        var payload = _jwtService.GetPayload(User);
        var duties = await _service.FindAllAsync(payload.Id);

        return Ok(duties);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> FindOne(Guid id)
    {
        var payload = _jwtService.GetPayload(User);
        var duty = await _service.FindOneAsync(id, payload.Id);

        return Ok(duty);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] SaveDutyDto dto)
    {
        var payload = _jwtService.GetPayload(User);
        await _service.UpdateAsync(id, payload.Id, dto);

        return NoContent();
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateDutyStatusDto dto)
    {
        var payload = _jwtService.GetPayload(User);
        await _service.UpdateStatusAsync(id, payload.Id, dto);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var payload = _jwtService.GetPayload(User);
        await _service.DeleteAsync(id, payload.Id);

        return NoContent();
    }
}
