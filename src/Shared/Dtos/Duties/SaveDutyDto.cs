namespace TaskManagement.src.Shared.Dtos.Duties;

using System.ComponentModel.DataAnnotations;
using TaskManagement.src.Domain.Models;

public record SaveDutyDto
{
    [Required, MaxLength(100)]
    public required string Name { get; set; }

    [Required]
    public required string Description { get; set; }

    public Duty ToEntity() => new()
    {
        Name = Name,
        Description = Description
    };
}
