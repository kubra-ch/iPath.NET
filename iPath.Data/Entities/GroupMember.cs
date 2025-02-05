namespace iPath.Data.Entities;

public class GroupMember : BaseEntity
{
    public int GroupId { get; set; }
    // public Group Group { get; set; }

    public int UserId { get; set; }
    // public User User { get; set; }

    public eMemberRole Role { get; set; }
}
