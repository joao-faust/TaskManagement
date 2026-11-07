namespace TaskManagement.src.Shared.Dtos.Users;

using System.ComponentModel.DataAnnotations;
using TaskManagement.src.Domain.Models;

public record SaveUserDto
{
    [Required, MaxLength(100)]
    public required string Name { get; set; }

    [Required, EmailAddress, MaxLength(254)]
    public required string Email { get; set; }
    
    [Required]
    public required string Password { get; set; }

    public User ToEntity() => new()
    {
        Name = Name,
        Email = Email,
        Password = Password
    };
}
