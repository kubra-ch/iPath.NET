using iPath.Application.Areas.Authentication;
using iPath.Application.Areas.Authentication.Commands;
using MediatR;

namespace iPath.UI.Areas.Authentication;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(string Username, string Password);
    Task<bool> LogoutAsync();
}

public class AuthServiceMediator (IMediator mediator) : IAuthService
{
    public Task<LoginResponse> LoginAsync(string Username, string Password)
    {
        var request = new PasswordLoginCommand(Username: Username, Password: Password);
        return mediator.Send(request);
    }

    public Task<bool> LogoutAsync()
    {
        return Task.FromResult(true);
    }
}
