using iPath.Data.Entities;

namespace iPath.Application.Events;

public class NodeUpdatedEvent : NotificationEvent
{
    public int NodeId { get; set; }
    public int? GroupId { get; set; }
    public int OwnerId { get; set; }

    public NodeUpdatedEvent()
    {
    }

    public NodeUpdatedEvent(Node node)
    {
        NodeId = node.Id;
        GroupId = node.GroupId;
        OwnerId = node.OwnerId;
    }
}
