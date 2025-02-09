using iPath.Application.Features;

namespace iPath.Application.Services;


public interface ISessionStateService
{
    public void Init(UserDto? user, bool isAdmin);
    UserDto? User { get; }
    bool IsAdmin { get; }
    bool IsAuthenticated { get; }
}

public class SessionStateService : ISessionStateService
{
    public void Init(UserDto? user, bool isAdmin)
    {
        User = user;
        isAdmin = isAdmin;
    }

    public UserDto? User { get; private set; }

    public bool IsAdmin { get; private set; }

    public bool IsAuthenticated => User != null;
}
