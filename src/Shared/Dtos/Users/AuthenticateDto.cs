using System.ComponentModel.DataAnnotations;

namespace TaskManagement.src.Shared.Dtos.Users;

public record AuthenticateDto
{
    [Required] 
    public required string Email { get; set; }

    [Required] 
    public required string Password { get; set; }
}
