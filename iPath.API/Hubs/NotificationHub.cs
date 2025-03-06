using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace iPath.Application.Hubs;


public class NotificationHub : Hub<NotificationMessage>
{
    public const string url = "api/hubs/notification";
    public const string method = "notification";
}


public class NotificationMessage
{
    public Guid Id { get; set; }
    public int? ReceiverUserId { get; set; }
    public int? NodeId { get; set; }
    public int? GroupId { get; set; }
    public DateTime CreatedOn { get; set; }
    public string Message { get; set; }
}