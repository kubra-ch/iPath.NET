using iPath.Application.Features;
using iPath.Data.Entities;

namespace iPath.UI.ViewModels.Admin.Users;

public interface IUserMemberViewModel
{
    Task<UserGroupMemberModel> LoadUserAsync(int Id);
    Task<List<GroupDto>> GetGroupList(int? communityId);

    UserGroupMemberModel SelectedUser { get; }
    Task<UserCommandResponse> SaveDataAsync();
}
