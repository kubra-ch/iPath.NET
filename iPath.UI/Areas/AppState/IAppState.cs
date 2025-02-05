using iPath.UI.Areas.Identity;

namespace iPath.UI.Areas.AppState;

public interface IAppState
{
    Task<UserSession> GetSessionAsync();
    UserSession Session { get; }
}
