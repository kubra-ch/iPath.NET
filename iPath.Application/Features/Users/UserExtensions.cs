using iPath.Data.Entities;

namespace iPath.Application.Features;

public static class UserExtensions
{
    public static UserDTO ToOwner(this User user)
    {
        return new UserDTO { UserId = user.Id, Username = user.Username, Initials = user.Profile?.Initials };
    }

    public static UserDTO ToOwner(this UserProfile profile)
    {
        return new UserDTO { UserId = profile.UserId, Username = profile.Username, Initials = profile.Initials };
    }
}
