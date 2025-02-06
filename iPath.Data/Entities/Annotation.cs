using Ardalis.SmartEnum;

namespace iPath.Data.Entities;

public class Annotation : BaseEntity
{
    public string? Text { get; set; }

    // Node is the main Node in the Group
    public int NodeId { get; set; }
    public Node? Node { get; set; }

    // reference to a specific SubNode => Image Annotation
    public int? SubNodeId { get; set; }


    public int? OwnerId { get; set; }
    public User? Owner { get; set; }

    public eAnnotationVisibility Visibility { get; set; }

    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
}



public sealed class eAnnotationVisibility : SmartEnum<eAnnotationVisibility>
{
    public static readonly eAnnotationVisibility Draft = new("Draft", 0);
    public static readonly eAnnotationVisibility Visible = new("Visible", 1);
    // public static readonly eAnnotationVisibility Public = new("Public", 2);
    public static readonly eAnnotationVisibility Hidden = new("Hidden", 10);
    public static readonly eAnnotationVisibility Deleted = new("Deleted", 11);
    public static readonly eAnnotationVisibility Blaimed = new("Blaimed", 12);
    private eAnnotationVisibility(string name, int id) : base(name, id) { }
}