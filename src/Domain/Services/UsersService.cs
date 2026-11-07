using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagement.src.Infrastructure.Database;
using TaskManagement.src.Shared.Dtos.Users;
using TaskManagement.src.Shared.Exceptions;
using TaskManagement.src.Domain.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManagement.src.Domain.Services;

public class UsersService(AppDbContext db, PasswordHasher<User> passwordHasher)
{
    private readonly AppDbContext _db = db;
    private readonly PasswordHasher<User> _passwordHasher = passwordHasher;

    public async Task<UserDetailDto> CreateAsync(SaveUserDto dto)
    {
        var user = dto.ToEntity();

        await EnsureEmailIsAvailableAsync(dto.Email);

        user.Password = _passwordHasher.HashPassword(user, user.Password);

        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();

        return UserDetailDto.FromEntity(user);
    }

    public async Task<UserDetailDto> AuthenticateAsync(AuthenticateDto dto)
    {
        var user = await _db.Users.SingleOrDefaultAsync(user => user.Email == dto.Email);

        if (user is null || !IsPasswordCorrect(user, dto.Password))
        {
            throw new InvalidStateException("Invalid credentials.");
        }

        return UserDetailDto.FromEntity(user);
    }

    public async Task<UserDetailDto> FindOneAsync(Guid id)
    {
        var user = await FindEntityAsync(id);
        return UserDetailDto.FromEntity(user);
    }

    public async Task UpdateAsync(Guid id, SaveUserDto dto)
    {
        var user = await FindEntityAsync(id);

        await EnsureEmailIsAvailableAsync(dto.Email, id);

        user.Name = dto.Name;
        user.Email = dto.Email;
        user.Password = _passwordHasher.HashPassword(user, dto.Password);

        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await FindEntityAsync(id);
        
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
    }

    public async Task EnsureExistsAsync(Guid id)
    {
        var result = await _db.Users.AnyAsync(user => user.Id == id);

        if (!result)
        {
            throw new NotFoundException($"Cannot find a user with the provided ID ({id}).");
        }
    }

    private async Task EnsureEmailIsAvailableAsync(string email, Guid? userId = null)
    {
        var emailIsInUse = await _db.Users.AnyAsync(user =>
            user.Email == email && (!userId.HasValue || user.Id != userId.Value));

        if (emailIsInUse)
        {
            throw new InvalidStateException("The provided email is already in use.");
        }
    }

    private bool IsPasswordCorrect(User user, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);

        return result == PasswordVerificationResult.Success;
    }

    private async Task<User> FindEntityAsync(Guid id)
    {
        return await _db.Users.FindAsync(id)
            ?? throw new NotFoundException($"Cannot find a user with the provided ID ({id}).");
    }
}
