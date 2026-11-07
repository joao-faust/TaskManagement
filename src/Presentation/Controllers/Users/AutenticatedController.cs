using Microsoft.AspNetCore.Mvc;
using TaskManagement.src.Domain.Services;
using TaskManagement.src.Shared.Dtos.Users;
using Microsoft.AspNetCore.Authorization;
using TaskManagement.src.Presentation.Services;

namespace TaskManagement.src.Presentation.Controllers.Users;

[ApiController, Authorize, Route("authenticated")]
public class MeController(UsersService service, JwtService jwtService) : ControllerBase
{
    private readonly UsersService _service = service;
    private readonly JwtService _jwtService = jwtService;

    [HttpGet]
    public async Task<IActionResult> FindDetails()
    {
        var payload = _jwtService.GetPayload(User);
        var user = await _service.FindOneAsync(payload.Id);

        return Ok(user);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] SaveUserDto dto)
    {
        var payload = _jwtService.GetPayload(User);
        await _service.UpdateAsync(payload.Id, dto);

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete()
    {
        var payload = _jwtService.GetPayload(User);
        await _service.DeleteAsync(payload.Id);

        return NoContent();
    }
}
