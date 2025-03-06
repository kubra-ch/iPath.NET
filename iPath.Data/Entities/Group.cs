using Ardalis.SmartEnum;

namespace iPath.Data.Entities;

public class Group: BaseEntityWithDeleteFlag
{
    public string Name { get; set; }

    public int? OwnerId { get; set; }
    public User? Owner { get; set; }

    public DateTime CreatedOn { get; set; }

    public eGroupType GroupType { get; set; } = eGroupType.DiscussionGroup;
    public eGroupVisibility Visibility { get; set; } = eGroupVisibility.MembersOnly;

    public ICollection<CommunityGroup> Communities { get; set; } = [];

    public ICollection<Node>? Nodes { get; set; }
    public ICollection<GroupMember>? Members { get; set; }

    public GroupSettings Settings { get; set; }
}


public class GroupSettings
{
    public string? Purpose { get; set; }
    public string? DefaultText { get; set; }
    public string? NodeDescriptionType { get; set; }
}





public class GroupMember : BaseEntity
{
    public int GroupId { get; set; }
    public Group Group { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public eMemberRole Role { get; set; }


    // User Preferences
    public bool IsFavourite { get; set; }
    public eNotification Notifications { get; set; }


    public override string ToString()
    {
        return $"g={GroupId}, u={UserId}";
    }
}


[Flags]
public enum eMemberRole
{
    None = 0,
    User = 1,
    Inactive = 2,
    Moderator = 4,
    Guest = 8
}



[Flags]
public enum eNotification
{
    None = 0,
    NewCase = 1,
    NewAnnotation = 2,
    NewAnnotationOnMyCase = 4
}



public sealed class eGroupVisibility : SmartEnum<eGroupVisibility>
{
    public static readonly eGroupVisibility Public = new("Public", 0);
    public static readonly eGroupVisibility MembersOnly = new("MembersOnly", 1);
    public static readonly eGroupVisibility Hidden = new("Hidden", 2);
    public static readonly eGroupVisibility Inactive = new("Inactive", 3);
    private eGroupVisibility(string name, int id) : base(name, id) { }
}


public sealed class eGroupType : SmartEnum<eGroupType>
{
    public static readonly eGroupType None = new("None", 0);
    public static readonly eGroupType DiscussionGroup = new("Discussion Group", 1);
    public static readonly eGroupType ExpertGroup = new("Expert Group", 2);
    public static readonly eGroupType TeachingGroup = new("Teaching Group", 3);
    public static readonly eGroupType PresentationGroup = new("Presentation Group", 4);

    private eGroupType(string name, int id) : base(name, id) { }
}
