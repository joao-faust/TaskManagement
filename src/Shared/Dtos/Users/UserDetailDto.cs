namespace TaskManagement.src.Shared.Dtos.Users;

using TaskManagement.src.Domain.Models;

public record UserDetailDto(Guid Id, string Name, string Email)
{
    public static UserDetailDto FromEntity(User user) => new(user.Id, user.Name, user.Email);
}
