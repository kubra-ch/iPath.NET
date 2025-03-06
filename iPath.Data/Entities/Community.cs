namespace iPath.Data.Entities;

public class Community : BaseEntityWithDeleteFlag
{
    public string Name { get; set; }
    public string? Description { get; set; }

    public DateTime CreatedOn { get; set; }

    public int OwnerId { get; set; }
    public User Owner { get; set; }


    public ICollection<CommunityGroup> Groups { get; set; } = [];
    public ICollection<CommunityMember>? Members { get; set; }

    public eCommunityVisibility Visibility { get; set; } = eCommunityVisibility.Public;
    public string? BaseUrl { get; set; }
}

public class CommunityMember : BaseEntity
{

    public int CommunityId { get; set; }
    public Community Community { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public eMemberRole Role { get; set; }
    public bool AllGroups { get; set; }
}

public class CommunityGroup
{
    public int Id { get; set; }
    public int CommunityId { get; set; }
    public Community Community { get; set; }
    public int GroupId { get; set; }
    public Group Group { get; set; }
}

public enum eCommunityVisibility
{
    Public = 0,
    MembersOnly = 1,
    Hidden = 2,
    Inactive = 3
}
