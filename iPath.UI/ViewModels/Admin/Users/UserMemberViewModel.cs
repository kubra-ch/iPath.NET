using iPath.Application.Features;
using iPath.Application.Querying;
using iPath.Data.Entities;
using iPath.UI.ViewModels.DataService;

namespace iPath.UI.ViewModels.Admin.Users;

public class UserMemberViewModel(IDataAccess srvData) : IUserMemberViewModel
{
    private UserGroupMemberModel _SelectedUser;
    public UserGroupMemberModel SelectedUser => _SelectedUser;

    public async Task<UserGroupMemberModel> LoadUserAsync(int Id)
    {
        // load existing data
        var response = await srvData.Send(new GetUserQuery(Id: Id));
        if( !response.Success ) throw new Exception(response.Message);

        var membership = await srvData.Send(new GetGroupMembershipQuery() { UserId = Id });
        _SelectedUser = new UserGroupMemberModel(response.Data, membership.Data);

        // create entries for all groups
        var allGroups = await GetGroupList(null);
        _SelectedUser.CreateMissingGroups(allGroups);

        return _SelectedUser;
    }

    public async Task<List<Group>> GetGroupList(Community community)
    {
        var request = new GetGroupListQuery() { Count = null, StartIndex = 0 };
        if (community != null)
        {
            request.CommunityId = community.Id;
        }
        request.SortDefinitions ??= new();
        request.SortDefinitions.Add(new SortDefinition("Name", true));
        var response = await srvData.Send(request);
        return  response.Success ? response.Data.Items : new List<Group>();
    }

    public async Task<UserCommandResponse> SaveDataAsync()
    {
        if (SelectedUser == null) return new UserCommandResponse(false, "no user selected");

        var request = new UpdateUserMembershipCommand() { Data = new(), UserId = SelectedUser.User.Id };
        foreach (var mb in SelectedUser.Membership )
        {
            if( mb.Role != eMemberRole.None)
            {
                request.Data.Add(new UserGroupMemberDto(GroupId: mb.GroupId, Role: mb.Role));
            }
        }

        return await srvData.Send(request);
    }
}
