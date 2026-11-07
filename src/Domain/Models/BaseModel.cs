namespace TaskManagement.src.Domain.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class BaseModel
{
    [Key]
    [Column("id")]
    public Guid Id { get; private set; } = Guid.NewGuid();

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
