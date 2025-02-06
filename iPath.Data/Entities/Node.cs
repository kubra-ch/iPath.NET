using Ardalis.SmartEnum;

namespace iPath.Data.Entities;

public class Node : BaseEntity
{
    public string? Title { get; set; }
    public string? SubTitle { get; set; }
    public string? Description { get; set; }

    public DateTime CreateOn { get; set; }
    public DateTime ModifiedOn { get; set; }

    public eNodeStatus Status { get; set; } = eNodeStatus.Open;
    public eNodeVisibility Visibility { get; set; } = eNodeVisibility.Visible;

    public NodeFile? File { get; set; }
        
    public int? SortNr { get; set; }

    public int? OwnerId { get; set; }
    public User? Owner { get; set; }

    public int? GroupId { get; set; }
    public Group? Group { get; set; } = null;

    public NodeType NodeType { get; set; }


    public ICollection<Annotation> Annotations { get; set; }
    public ICollection<NodeField> Fields { get; set; }


    public int? ParentNodeId { get; set; }
    public int? TopNodeId { get; set; }
    public ICollection<Node> ChildNodes { get; set; }
}


public sealed class eNodeStatus : SmartEnum<eNodeStatus>
{
    public static readonly eNodeStatus None = new("None", 0);
    public static readonly eNodeStatus Closed = new("Closed", 1);
    public static readonly eNodeStatus Review = new("Review", 2);
    public static readonly eNodeStatus Open = new("Open", 3);
    private eNodeStatus(string name, int id) : base(name, id) { }
}


public sealed class eNodeVisibility : SmartEnum<eNodeVisibility>
{
    public static readonly eNodeVisibility Draft = new("Draft", 0);
    public static readonly eNodeVisibility Visible = new("Visible", 1);
    public static readonly eNodeVisibility Public = new("Public", 2);
    public static readonly eNodeVisibility Hidden = new("Hidden", 10);
    public static readonly eNodeVisibility Deleted = new("Deleted", 11);
    private eNodeVisibility(string name, int id) : base(name, id) { }
}