using iPath.Data.Helper;
using MediatR;

namespace iPath.Application.Events;

public class NotificationEvent : INotification
{
    public Guid Id { get; set; } = SequentialGuidUtility.GetGuid();
    public DateTime Date { get; set; }

    // the group in which the event was triggered
    public int? GroupId { get; set; }

    // the node in which the event was triggered
    public int? NodeId { get; set; }

    // the user who triggered the event
    public int UserId { get; set; }
}
