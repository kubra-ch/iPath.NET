using iPath.Application.Features;
using Microsoft.Identity.Client;

namespace iPath.UI.Areas.Identity;

public record UserSession(
    UserDto User,
    string Role
)
{
    public int UserId => User.Id;
    public bool IsAdmin => User.IsSysAdmin;
}
