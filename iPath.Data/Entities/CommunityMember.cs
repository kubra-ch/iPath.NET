namespace iPath.Data.Entities;

public class CommunityMember : BaseEntity
{
    public int CommunityId { get; set; }
    public Community Community { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public eMemberRole Role { get; set; }
    public bool AllGroups { get; set; }
}
