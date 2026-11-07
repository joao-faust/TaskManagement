namespace TaskManagement.src.Domain.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[Table("duties")]
public class Duty : BaseModel
{
    [Column("name"), MaxLength(100)]
    public required string Name { get; set; }

    [Column("description"), MaxLength(500)]
    public required string Description { get; set; }

    [Column("status")]
    public DutyStatus Status { get; set; } = DutyStatus.Pending;

    [Column("cancellation_reason"), MaxLength(500)]
    public string? CancellationReason { get; set; }

    [Column("concluded_at")]
    public DateTime? ConcludedAt { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [DeleteBehavior(DeleteBehavior.Restrict)]
    public User User { get; set; } = null!;
}
