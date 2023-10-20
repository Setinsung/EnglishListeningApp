using IdentityService.Domain.Entities;

namespace IdentityService.WebAPI.Controllers.UserAdmin;

public record UserDTO(Guid Id, string? UserName, string? PhoneNumber, DateTime CreatedTime)
{
    public static UserDTO Create(User user)
    {
        return new UserDTO(user.Id, user.UserName, user.PhoneNumber, user.CreatedTime);
    }
}
