using iPath.Application.Features;
using iPath.Application.Querying;
using iPath.Data.Entities;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;

namespace iPath.UI.ViewModels.Admin.Users;

public class UserMemberViewModel(IMediator mediator) : IUserMemberViewModel
{
    private UserGroupMemberModel _SelectedUser;
    public UserGroupMemberModel SelectedUser => _SelectedUser;

    public async Task<UserGroupMemberModel> LoadUserAsync(int Id)
    {
        // load existing data
        var usr = await mediator.Send(new GetUserQuery() { Id = Id });
        var membership = await mediator.Send(new GetGroupMembershipQuery() { UserId = Id });
        _SelectedUser = new UserGroupMemberModel(usr, membership);

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
        var data = await mediator.Send(request);
        return data.Items;
    }

    public async Task<UpdateUserMembershipResponse> SaveDataAsync()
    {
        if (SelectedUser == null) return new UpdateUserMembershipResponse(false, "no user selected");

        var request = new UpdateUserMembershipCommand() { Data = new(), UserId = SelectedUser.User.Id };
        foreach (var mb in SelectedUser.Membership )
        {
            if( mb.Role != eMemberRole.None)
            {
                request.Data.Add(new UserGroupMemberDto(GroupId: mb.GroupId, Role: mb.Role));
            }
        }

        return await mediator.Send(request);
    }
}
