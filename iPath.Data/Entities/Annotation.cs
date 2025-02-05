namespace iPath.Data.Entities;

public class Annotation : BaseEntity
{
    public string Text { get; set; }

    public int NodeId { get; set; }
    public Node? Node { get; set; }

    public int? OwnerId { get; set; }
    public User? Owner { get; set; }

    public DateTime CreatedOn { get; set; }
}
