using Ardalis.SmartEnum;
using System.ComponentModel.DataAnnotations.Schema;

namespace iPath.Data.Entities;

public class Group : BaseEntity
{
    public string Name { get; set; }
    public string? Purpose { get; set; }

    public int? OwnerId { get; set; }
    public User? Owner { get; set; }

    public eGroupVisibility Visibility { get; set; } = eGroupVisibility.MembersOnly;
    public GroupSetting? Settings { get; set; } = new();

    public int? CommunityId { get; set; }
    public Community? Community { get; set; }

    public ICollection<Node>? Nodes { get; set; }
    public ICollection<GroupMember>? Members { get; set; }
}


public sealed class eGroupVisibility : SmartEnum<eGroupVisibility>
{
    public static readonly eGroupVisibility Public = new("Public", 0);
    public static readonly eGroupVisibility MembersOnly = new("MembersOnly", 1);
    public static readonly eGroupVisibility Hidden = new("Hidden", 2);
    public static readonly eGroupVisibility Inactive = new("Inactive", 3);
    private eGroupVisibility(string name, int id) : base(name, id) { }
}


[ComplexType]
public class GroupSetting
{
    public List<string> NodeTypes { get; set; } = new();
}
