using iPath.Data.Entities;
using System.Security.Cryptography.X509Certificates;

namespace iPath.UI.ViewModels.Admin.Users;

public class UserGroupMemberModel
{
    public User User { get; set; }
    public List<MemberRoleModel> Membership { get; set; } = new();

    public UserGroupMemberModel(User usr, List<GroupMember> member)
    {
        User = usr;
        foreach (var mb in member)
        {
            Membership.Add(new MemberRoleModel(mb));
        }
    }

    public void CreateMissingGroups(List<Group> groups)
    {
        foreach (var group in groups)
        {
            var dto = Membership.FirstOrDefault(m => m.GroupId == group.Id);
            if (dto == null)
            {
                dto = new MemberRoleModel(group.CommunityId, group.Id, group.Name, eMemberRole.None);
                Membership.Add(dto);
            }
            dto.Groupname = group.Name;
            dto.CommunityId  = group.CommunityId;
        }
    }
}

public class MemberRoleModel
{
    public int? CommunityId { get; set; }
    public int GroupId { get; set; }    
    public string Groupname { get; set; }

    public bool Guest { get; set; }
    public bool User { get; set; }
    public bool Moderator { get; set; }
    public bool Admin { get; set; }

    public eMemberRole Role => (Admin ? eMemberRole.Admin : eMemberRole.None) |
         (Moderator ? eMemberRole.Moderator : eMemberRole.None) |
         (User ? eMemberRole.User : eMemberRole.None) |
         (Guest ? eMemberRole.Guest : eMemberRole.None);

    public MemberRoleModel(int? communityId, int groupId, string groupname, eMemberRole role)
    {
        CommunityId = communityId;
        GroupId = groupId;
        Groupname = groupname;

        Guest = role.HasFlag(eMemberRole.Guest);
        User = role.HasFlag(eMemberRole.User);
        Moderator = role.HasFlag(eMemberRole.Moderator);
        Admin = role.HasFlag(eMemberRole.Admin);
    }

    public MemberRoleModel(GroupMember mb)
    {
        GroupId = mb.GroupId;

        Guest = mb.Role.HasFlag(eMemberRole.Guest);
        User = mb.Role.HasFlag(eMemberRole.User);
        Moderator = mb.Role.HasFlag(eMemberRole.Moderator);
        Admin = mb.Role.HasFlag(eMemberRole.Admin);
    }

    public void Clear(bool isAdmin)
    {
        Guest = false;
        User = false;
        Moderator = false;
        if (isAdmin)  Admin = false;
    }
}