using iPath.Application.Features;
using iPath.Application.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace iPath.UI.Areas.Identity;

public class iPathAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedLocalStorage _sessionStorage;
    private readonly ISessionStateService _sessionState;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public iPathAuthenticationStateProvider(ProtectedLocalStorage SessionStorage, ISessionStateService sessionState)
    {
        _sessionStorage = SessionStorage;
        _sessionState = sessionState;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var userSession = await GetSessionAsync();
            if (userSession is null)
            {
                _sessionState.Init(null, false);
                return await Task.FromResult(new AuthenticationState(_anonymous));
            }
            _sessionState.Init(userSession.User, userSession.IsAdmin);
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, userSession.User.Username),
                new Claim(ClaimTypes.Email, userSession.User.Email),
                new Claim(ClaimTypes.Role, userSession.Role)
            }, "iPathAuth"));
            return await Task.FromResult(new AuthenticationState(claimsPrincipal));
        }
        catch (Exception ex)
        {
            return await Task.FromResult(new AuthenticationState(_anonymous));
        }
    }

    public async Task UpdateAuthentication(UserSession userSession)
    {
        ClaimsPrincipal principal;
        if (userSession is not null)
        {
            await _sessionStorage.SetAsync("UserSession", userSession);
            principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, userSession.User.Username),
                new Claim(ClaimTypes.Email, userSession.User.Email),
                new Claim(ClaimTypes.Role, userSession.Role)
            }, "iPathAuth"));
        }
        else
        {
            await _sessionStorage.DeleteAsync("UserSession");
            principal = _anonymous;
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    public async Task<UserSession> GetSessionAsync()
    {
        var userSessionResult = await _sessionStorage.GetAsync<UserSession>("UserSession");
        return userSessionResult.Success ? userSessionResult.Value : null!;
    }
}
