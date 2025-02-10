using iPath.Application.Features;
using iPath.UI.ViewModels.Drafts;

namespace iPath.UI.ViewModels.Users;

public interface IUserProfileViewModel
{
    Task LoadUserAsync(int UserId);
    UserModel Model { get; }

    Task<UserCommandResponse> SaveModelAsync();
}
