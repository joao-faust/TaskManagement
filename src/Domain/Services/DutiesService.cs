using Microsoft.EntityFrameworkCore;
using TaskManagement.src.Infrastructure.Database;
using TaskManagement.src.Shared.Dtos.Duties;
using TaskManagement.src.Shared.Exceptions;
using TaskManagement.src.Domain.Models;
namespace TaskManagement.src.Domain.Services;

public class DutiesService(AppDbContext db, UsersService usersService)
{
    private readonly AppDbContext _db = db;
    private readonly UsersService _usersService = usersService;

    public async Task<DutyDetailDto> CreateAsync(Guid userId, SaveDutyDto dto)
    {
        await _usersService.EnsureExistsAsync(userId);

        var duty = dto.ToEntity();
        duty.UserId = userId;

        await _db.Duties.AddAsync(duty);
        await _db.SaveChangesAsync();

        return DutyDetailDto.FromEntity(duty);
    }

    public async Task<DutyDetailDto> FindOneAsync(Guid id, Guid userId)
    {
        var duty = await FindEntityAsync(id, userId);
        return DutyDetailDto.FromEntity(duty);
    }

    private async Task<Duty> FindEntityAsync(Guid id, Guid userId)
    {
        await _usersService.EnsureExistsAsync(userId);

        var duty = await _db.Duties.FindAsync(id);

        if (duty is null || userId != duty.UserId)
        {
            throw new NotFoundException($"Cannot find a duty with the provided ID ({id}).");
        }

        return duty;
    }

    public async Task<List<DutyDetailDto>> FindAllAsync(Guid userId)
    {
        await _usersService.EnsureExistsAsync(userId);

        var duties = await _db.Duties.Where(duty => duty.UserId == userId).ToListAsync();
        return DutyDetailDto.FromEntities(duties);
    }

    public async Task UpdateAsync(Guid id, Guid userId, SaveDutyDto dto)
    {
        var duty = await FindEntityAsync(id, userId);

        if (duty.Status is not (DutyStatus.Pending or DutyStatus.InProgress))
        {
            throw new InvalidStateException("Only pending or in-progress duties can be edited.");
        }

        duty.Name = dto.Name;
        duty.Description = dto.Description;

        _db.Duties.Update(duty);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(Guid id, Guid userId, UpdateDutyStatusDto dto)
    {
        var duty = await FindEntityAsync(id, userId);

        var isValidTransition = (duty.Status, dto.Status) is
            (DutyStatus.Pending, DutyStatus.InProgress) or
            (DutyStatus.InProgress, DutyStatus.Concluded) or
            (DutyStatus.Pending, DutyStatus.Canceled) or
            (DutyStatus.InProgress, DutyStatus.Canceled) or
            (DutyStatus.Concluded, DutyStatus.Pending) or
            (DutyStatus.Canceled, DutyStatus.Pending);

        if (!isValidTransition)
        {
            throw new InvalidStateException(
                $"Cannot change duty status from {duty.Status} to {dto.Status}.");
        }

        duty.Status = dto.Status;
        duty.CancellationReason = null;

        if (dto.Status == DutyStatus.Concluded)
        {
            duty.ConcludedAt = DateTime.UtcNow;
        }
        else if (dto.Status == DutyStatus.Pending)
        {
            duty.ConcludedAt = null;
        }
        else if (dto.Status == DutyStatus.Canceled)
        {
            duty.CancellationReason = dto.CancellationReason;
        }

        _db.Duties.Update(duty);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var duty = await FindEntityAsync(id, userId);

        if (duty.Status is not (DutyStatus.Pending or DutyStatus.Canceled))
        {
            throw new InvalidStateException("Only pending or canceled duties can be deleted.");
        }

        _db.Duties.Remove(duty);
        await _db.SaveChangesAsync();
    }
}
