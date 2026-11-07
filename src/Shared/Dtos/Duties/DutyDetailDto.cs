namespace TaskManagement.src.Shared.Dtos.Duties;

using TaskManagement.src.Domain.Models;

public record DutyDetailDto(
    Guid Id,
    string Name,
    string Description,
    DutyStatus Status,
    string? CancellationReason,
    DateTime? ConcludedAt)
{
    public static DutyDetailDto FromEntity(Duty duty) => new(
        duty.Id,
        duty.Name,
        duty.Description,
        duty.Status,
        duty.CancellationReason,
        duty.ConcludedAt);

    public static List<DutyDetailDto> FromEntities(IEnumerable<Duty> duties) =>
        [.. duties.Select(FromEntity)];
}
