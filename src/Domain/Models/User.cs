namespace TaskManagement.src.Domain.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("users")]
public class User : BaseModel
{
    [Column("name")]
    [MaxLength(100)]
    public required string Name { get; set; }

    [Column("email")]
    [MaxLength(254)]
    public required string Email { get; set; }

    [Column("password")]
    [MaxLength(255)]
    public required string Password { get; set; }

    public ICollection<Duty> Duties { get; set; } = [];
}
