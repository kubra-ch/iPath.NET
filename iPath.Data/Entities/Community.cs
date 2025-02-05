namespace iPath.Data.Entities;


public class Community : BaseEntity
{
    public string Name { get; set; }
    public string? Descritption { get; set; }

    public ICollection<Group>? Groups { get; set; }
    public ICollection<CommunityMember>? Members { get; set; }

    public eCommunityVisibility Visibility { get; set; } = eCommunityVisibility.Public;
        
    public CommunitySetting? Settings { get; set; } = new();
}


public enum eCommunityVisibility
{
    Public = 0,
    MembersOnly = 1,
    Hidden = 2,
    Inactive = 3
}


public class CommunitySetting
{
    public string? TestSettings { get; set; }
    public List<string> Tags { get; set; } = new();
}

