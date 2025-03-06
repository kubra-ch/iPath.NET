using iPath.Data.Entities;
using iPath.Data.Helper;
using iPath.Application.Features;

namespace iPath.Application.Events;

public class AnnotationAddedEvent : NotificationEvent
{
    // properties of parent node => necessary for filterung
    public int NodeId { get; set; }
    public int AnnotationId { get; set; }
    public int NodeOwnerId { get; set; }

    // annotation properties
    public Guid? ChildNodeId { get; set; }

    public string Text { get; set; }


    public AnnotationAddedEvent()
    {
    }


    public AnnotationAddedEvent(Annotation annotation, Node parent)
    {
        if (parent is null || !parent.GroupId.HasValue) throw new Exception("Annotation Events are allowed only on nodes that belong to a Group");

        Id = SequentialGuidUtility.GetGuid();

        NodeId = parent.Id;
        UserId = annotation.Owner.Id;
        GroupId = parent.GroupId.Value;
        NodeOwnerId = parent.OwnerId;

        AnnotationId = annotation.Id;
        Date = annotation.CreatedOn;
        Text = annotation.Text;
    }
}
