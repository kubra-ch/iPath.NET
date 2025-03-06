using iPath.Application.Events;
using iPath.Application.Hubs;
using iPath.Application.Services.Cache;
using iPath.Data;
using iPath.Data.Entities;
using iPath.Data.Helper;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using static System.Net.Mime.MediaTypeNames;

namespace iPath.API.Services;


public class NotificationService(IServiceProvider serviceProvider,
    IHubContext<NotificationHub> hub,    
    ILogger<NotificationService> logger) : BackgroundService
{
    private ConcurrentQueue<NotificationEvent> notificationEcents = new();

    public void AddEvent(NotificationEvent e)
    {
        notificationEcents.Enqueue(e);
    }

    private IDataCache srvCache;

    protected override async Task ExecuteAsync(CancellationToken ctk)
    {
        while (!ctk.IsCancellationRequested)
        {
            if (notificationEcents.Count > 0)
            {
                try
                {
                    using var scope = serviceProvider.CreateScope();
                    using var dbContext = scope.ServiceProvider.GetService<NewDB>();
                    srvCache = scope.ServiceProvider.GetService<IDataCache>();
                    var processed = 0;
                    while (notificationEcents.TryDequeue(out var e))
                    {
                        await ProcessEvent(e, dbContext, ctk);
                        processed++;
                    }
                    logger.LogInformation("processed {n} events", processed);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                }
                srvCache = null;
            }

            await Task.Delay(1000, ctk);
        }
    }


    private async Task ProcessEvent(NotificationEvent e, NewDB dbContext, CancellationToken ctk)
    {
        // system events => GroupId has no value => just send to SignalR
        if( !e.GroupId.HasValue )
        {
            await SendToSignalR(e, e.ToString(), null, ctk);
            return;
        }


        // get event consumers for this group
        var list = await dbContext.Set<GroupMember>().AsNoTracking()
            .Where(m => m.GroupId == e.GroupId.Value && m.Notifications != eNotification.None)
            .Select(m => new { m.UserId, m.Notifications })
            .ToListAsync();

        foreach (var m in list)
        {
            // skip if trigger is the own user
            if (m.UserId != e.UserId)
            {
                if( UserSubscribedEventNotification(e, m.UserId, m.Notifications) )
                {
                    var text = await GetEventText(e);

                    if (!string.IsNullOrEmpty(text))
                    {
                        // Save to DB
                        var entity = new UserNotification
                        {
                            Type = eNotificationType.NewAnnotation,
                            UserId = m.UserId,
                            Message = text,
                            Date = DateTime.UtcNow
                        };
                        await dbContext.Set<UserNotification>().AddAsync(entity);
                        await dbContext.SaveChangesAsync();

                        // send to signalr
                        await SendToSignalR(e, text, m.UserId, ctk);

                        // trigger other services (email, etc)
                    }
                }
            }
        }
    }


    private bool UserSubscribedEventNotification(NotificationEvent e, int userId, eNotification notifyFflags)
    {
        bool subscribed = false;
        if( e is AnnotationAddedEvent)
        {
            if (notifyFflags.HasFlag(eNotification.NewAnnotation))
            {
                subscribed = true;
            }
            else if (notifyFflags.HasFlag(eNotification.NewAnnotationOnMyCase) && userId == (e as AnnotationAddedEvent).NodeOwnerId)
            {
                subscribed = true;
            }
        }
        else if (e is NodeCreatedEvent)
        {
            if( notifyFflags.HasFlag(eNotification.NewCase))
            {
                subscribed = true;
            }
        }
        return subscribed;
    }


    private async Task<string> GetEventText(NotificationEvent e)
    {
        var profile = await srvCache.GetProfileAsync(e.UserId);
        var group = await srvCache.GetGroupDtoAsync(e.GroupId.Value);

        if (e is AnnotationAddedEvent)
        {
            var ae = e as AnnotationAddedEvent;
            return $"{profile.Username} commented on case #{ae.NodeId}:<br />" + ae.Text.ShortenTo(100);
        }
        else if (e is NodeCreatedEvent)
        {
            var ne = e as NodeCreatedEvent;
            return $"{profile.Username} added a new case #{ne.NodeId}:<br />Group: {group.Name}";
        }
        return string.Empty;
    }


    private async Task SendToSignalR(NotificationEvent e, string text, int? UserId = null, CancellationToken ctk = default)
    {
        // send to signalr
        var msg = new NotificationMessage
        {
            Id = e.Id,
            ReceiverUserId = UserId,
            CreatedOn = e.Date,
            NodeId = e.NodeId,
            Message = text
        };
        await hub.Clients.All.SendAsync(NotificationHub.method, msg, ctk);
    }
}



public class AnnotationAddedConsumer(NotificationService srv) : INotificationHandler<AnnotationAddedEvent>
{

    public Task Handle(AnnotationAddedEvent e, CancellationToken cancellationToken)
    {
        srv.AddEvent(e);
        return Task.CompletedTask;
    }
}


public class NodeCreatedEventConsumer(NotificationService srv) : INotificationHandler<NodeCreatedEvent>
{
    public Task Handle(NodeCreatedEvent e, CancellationToken cancellationToken)
    {
        srv.AddEvent(e);
        return Task.CompletedTask;
    }
}
