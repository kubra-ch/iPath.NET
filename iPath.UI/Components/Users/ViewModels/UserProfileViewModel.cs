using Hl7.Fhir.Utility;
using iPath.Application.Authentication;
using iPath.Application.Features;
using iPath.Application.Services.Cache;
using iPath.Data.Entities;
using iPath.UI.Areas.DataAccess;
using MudBlazor;

namespace iPath.UI.Components.Users.ViewModels;

public class UserProfileViewModel(IDataAccess srvData, IDataCache srvCache, IAuthManager authManager, ISnackbar snackbar)
{
    private User _user;

    public User User => _user;

    private UserProfile _model;
    public UserProfile Model => _model;
    
    public ContactDetails Contact => Model.ContactDetails.FirstOrDefault(x => x.IsMainContact);


    public async Task<GetUserResponse> LoadUserAsync(int UserId)
    {
        _user = null;
        _model = null;
        var resp = await srvData.Send(new GetUserQuery(Id:  UserId));
        if (!resp.Success)
        {
            snackbar.Add(resp.Message, Severity.Error);
            return resp;
        }
            
        _user = resp.Data;
        _user.Profile ??= new();
        if( !_user.Profile.ContactDetails.Any())
        {
            _user.Profile.ContactDetails.Add(new ContactDetails
            {
                IsMainContact = true,
                Address = new()
            });
        }
        _model = _user.Profile;
        return resp;
    }

    public void LoadNewProfileModel()
    {
        _model = UserProfile.EmptyProfile();
    }



    public async Task<UserCommandResponse> SaveProfileAsync()
    {
        if( _user != null )
        {
            return await srvData.Send(new UpdateUserProfileCommand(UserId: _user.Id, Profile: Model));
        }
        return new UserCommandResponse(false);
    }




    private bool IsAdmin => true;

    public async Task<UserCommandResponse> UpdateUsername(string newUsername)
    {
        if (!IsAdmin || _user is null) return new UserCommandResponse(false);
        return await srvData.Send(new UpdateUsernameCommand(UserId: _user.Id, Username: newUsername));
    }

    public async Task<UserCommandResponse> UpdateEmail(string newEmail)
    {
        if (!IsAdmin || _user is null) return new UserCommandResponse(false);
        return await srvData.Send(new UpdateUserEmailCommand(UserId: _user.Id, Email: newEmail));
    }

    public async Task<UserCommandResponse> UpdatePassword(string newPassword)
    {
        if (!IsAdmin || _user is null) return new UserCommandResponse(false);
        return await srvData.Send(new UpdateUserPasswordCommand(UserId: _user.Id, newPassword: newPassword, IsActive: true));
    }



    public async Task<List<GroupMemberModel>> GetMembershipsAsync(bool ActiveOnly)
    {
        if (User is null) return new();

        var resp = await srvData.Send(new GetUserMembershipQuery(UserId: User.Id));
        if (!resp.Success) return null;

        var profile = await authManager.GetProfileAync();

        IEnumerable<GroupDTO> groups = null!;
        // GroupListResponse respGroups = null!;
        if (!ActiveOnly)
        {
            if (authManager.IsAdmin())
            {
                // get ALL groups for admins
                var rx = await srvData.Send(new GetGroupListQuery());
                groups = rx.Data;
            }
            else if (authManager.IsModerator())
            {
                // get moderators group list
                var rx = await srvData.Send(new GetGroupListQuery { ModeratorId = profile.UserId});
                groups = rx.Data;
            }
        }
        else
        {
            var list = new List<GroupDTO>();
            foreach (var m in resp.Data.GroupMembership)
            {
                var grp = await srvCache.GetGroupDtoAsync(m.GroupId);
                list.Add(grp);
            }
            groups = list.ToArray();
        }

        if (groups is null) return null;
       
        var usr = resp.Data;

        var tmp = new List<GroupMemberModel>();
        foreach (var g in groups)
        {
            var gm = new GroupMemberModel { GroupId = g.Id, Groupname = g.Name };
            gm.member = usr.GroupMembership.FirstOrDefault(m => m.GroupId == g.Id);
            gm.member ??= new GroupMember { GroupId = g.Id, UserId = usr.Id, Role = eMemberRole.None };
            tmp.Add(gm);
        }

        // remove all memberships that are NOT in groups list
        tmp.RemoveAll(m => !groups.Select(g => g.Id).Contains(m.GroupId));

        return tmp.OrderBy(g => g.Groupname).ToList();
    }

    public async Task<bool> SaveMembershipAsync(List<GroupMemberModel> data)
    {
        if (User is null) return false;

        // convert to dto
        var dto = data.Select(x => new GroupMemberShipDto(x.GroupId, x.Role)).ToList();

        // remove empty (don't remove on the original list as this is bound to the UI)
        dto.RemoveAll(n => n.Role == eMemberRole.None);

        // send to server
        var resp = await srvData.Send(new UpdateGroupMembershipCommand(UserId: User.Id, Membership: dto.ToArray()));
        return resp.Success;
    }



    public async Task<bool> SaveNotificationsAsync(List<GroupMemberModel> data)
    {
        if( User is null ) return false;

        // convert to dto
        var dto = data.Select(x => new GroupNotificationDto(x.GroupId, x.Notifications)).ToList();

        // remove empty (don't remove on the original list as this is bound to the UI)
        dto.RemoveAll(n => n.Notifications == eNotification.None);

        // send to server
        var resp = await srvData.Send(new UpdateNotificationsCommand(UserId: User.Id, Notifications: dto.ToArray()));
        return resp.Success;
    }



    public async Task<UserCommandResponse> RegisterUser(string username, string password, string email)
    {
        if( Model.UserId > 0)
        {
            return new UserCommandResponse(false, "User Profile is already assigned to UserId " + Model.UserId.ToString());
        }

        // create account
        var resp = await srvData.Send(new CreateUserCommand(Username: username, Password: password, Email: email));
        if (!resp.Success) return resp;


        // update profile
        Model.UserId = resp.Data.Id;
        Model.Username = resp.Data.Username;
        return await srvData.Send(new UpdateUserProfileCommand(UserId: resp.Data.Id, Profile: Model));
    }
}
