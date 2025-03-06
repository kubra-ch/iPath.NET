using iPath.Data.Entities;
using MediatR;

namespace iPath.Application.Events;

public class NodeCreatedEvent : NotificationEvent
{
    public int NodeId { get; set; }

    public NodeCreatedEvent()
    {
    }

    public static NodeCreatedEvent? CreateNodeEvent(Node node)
    {
        if (!node.GroupId.HasValue) return null!;

        return new NodeCreatedEvent
        {
            NodeId = node.Id,
            UserId = node.OwnerId,
            GroupId = node.GroupId.Value,
            Date = node.CreatedOn
        };
    }
}
