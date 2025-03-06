using iPath.Application.Events;
using iPath.Application.Services.Storage;
using iPath.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace iPath.Application.Services;



public class NodeJsonExportService(IServiceProvider serviceProvider,
    ILogger<NodeJsonExportService> logger) : BackgroundService
{
    private ConcurrentQueue<NodeUpdatedEvent> queuedEvents = new();

    public void AddEvent(NodeUpdatedEvent e)
    {
        queuedEvents.Enqueue(e);
    }

    protected override async Task ExecuteAsync(CancellationToken ctk)
    {
        while (!ctk.IsCancellationRequested)
        {
            if (queuedEvents.Count > 0)
            {
                try
                {
                    using var scope = serviceProvider.CreateScope();
                    var srvStorage = scope.ServiceProvider.GetService<IStorageService>();
                    var processed = new HashSet<int>();
                    while (queuedEvents.TryDequeue(out var e))
                    {
                        if( !processed.Contains(e.NodeId))
                        {
                            await srvStorage.PutNodeJsonAsync(e.NodeId, ctk);
                            processed.Add(e.NodeId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                }
            }

            await Task.Delay(5000, ctk);
        }
    }
}




public class NodeUpdatedEventConsumer(NodeJsonExportService srv) : INotificationHandler<NodeUpdatedEvent>
{

    public Task Handle(NodeUpdatedEvent @event, CancellationToken cancellationToken)
    {
        srv.AddEvent(@event);
        return Task.CompletedTask;
    }
}
