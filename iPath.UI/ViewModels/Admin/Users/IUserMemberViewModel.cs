using iPath.Application.Features;
using iPath.Data.Entities;

namespace iPath.UI.ViewModels.Admin.Users;

public interface IUserMemberViewModel
{
    Task<UserGroupMemberModel> LoadUserAsync(int Id);
    Task<List<Group>> GetGroupList(Community community);

    UserGroupMemberModel SelectedUser { get; }
    Task<UpdateUserMembershipResponse> SaveDataAsync();
}
