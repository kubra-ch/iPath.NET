namespace iPath.Data.Entities;
public class NodeField : BaseEntity
{
    public int NodeId { get; set; }
    public Node Node { get; set; }

    public string Field { get; set; }
    public string? Text { get; set; }
    public string? Coding_Code { get; set; }
    public string? Coding_System { get; set; }
}
