using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.src.Shared.Dtos.Users;
using TaskManagement.src.Shared.Exceptions;
using TaskManagement.src.Domain.Services;
using TaskManagement.src.Presentation.Services;

namespace TaskManagement.src.Presentation.Controllers.Users;

[ApiController, AllowAnonymous, Route("users")]
public class UsersController(UsersService service, JwtService jwtService) : ControllerBase
{
    private readonly UsersService _service = service;
    private readonly JwtService _jwtService = jwtService;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SaveUserDto dto)
    {
        await _service.CreateAsync(dto);

        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPost("tokens")]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateDto dto)
    {
        try
        {
            var user = await _service.AuthenticateAsync(dto);

            return Ok(new { token = _jwtService.Generate(user) });
        }
        catch (InvalidStateException exception)
        {
            return Unauthorized(new { message = exception.Message });
        }
    }
}
