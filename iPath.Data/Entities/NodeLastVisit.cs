namespace iPath.Data.Entities;

public class NodeLastVisit
{
    public int UserId { get; set; }
    public User User { get; set; }

    public int NodeId { get; set; }
    public Node Node { get; set; }

    public DateTime Date { get; set; }
}
