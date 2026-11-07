namespace TaskManagement.src.Shared.Dtos.Duties;

using System.ComponentModel.DataAnnotations;
using TaskManagement.src.Domain.Models;

public record UpdateDutyStatusDto : IValidatableObject
{
    [Required]
    public required DutyStatus Status { get; set; }

    public string? CancellationReason { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Status == DutyStatus.Canceled && string.IsNullOrWhiteSpace(CancellationReason))
        {
            yield return new ValidationResult(
                "A cancellation reason is required.", [nameof(CancellationReason)]);
        }
    }
}
