using iPath.Data.Entities;

namespace iPath.UI.Components.Users.ViewModels;

public class GroupMemberModel
{
    public GroupMember member { get; set; }
    public string Groupname { get; set; }
    public int GroupId { get; set; }

    public bool IsMember
    {
        get => member.Role.HasFlag(eMemberRole.User);
        set => member.Role = value ? eMemberRole.User : eMemberRole.None;
    }

    public bool IsModerator
    {
        get => member.Role.HasFlag(eMemberRole.Moderator);
        set => member.Role = value ? eMemberRole.Moderator : eMemberRole.None;
    }

    public bool IsGuest
    {
        get => member.Role.HasFlag(eMemberRole.Guest);
        set => member.Role = value ? eMemberRole.Guest : eMemberRole.None;
    }


    public bool NotifyNewCase
    {
        get => member.Notifications.HasFlag(eNotification.NewCase);
        set => member.Notifications ^= eNotification.NewCase;
    }
    public bool NotifyNewAnnotation
    {
        get => member.Notifications.HasFlag(eNotification.NewAnnotation);
        set => member.Notifications ^= eNotification.NewAnnotation;
    }
    public bool NotifyNewAnnotationOnMyCase
    {
        get => member.Notifications.HasFlag(eNotification.NewAnnotationOnMyCase);
        set => member.Notifications ^= eNotification.NewAnnotationOnMyCase;
    }
    public void ClearAll()
    {
        member.Notifications = eNotification.None;
    }


    public eMemberRole Role => member.Role;
    public eNotification Notifications => member.Notifications;
}