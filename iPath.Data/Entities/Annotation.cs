namespace iPath.Data.Entities;

public class Annotation : BaseEntityWithDeleteFlag
{
    public int? NodeId { get; set; }
    public Node Node { get; set; }


    public DateTime CreatedOn { get; set; }

    public int OwnerId { get; set; }
    public User Owner { get; set; }
    public string? Text { get; set; }
}
