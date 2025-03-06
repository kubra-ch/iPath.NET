using iPath.Application.Hubs;
using iPath.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using iPath.Application.Authentication;

namespace iPath.API;

public static class DI_Api
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {

        // Mediator
        services.AddMediatR(opts => opts.RegisterServicesFromAssembly(typeof(DI_Api).Assembly));

        // Notification Services
        services.AddSingleton<NotificationService>();
        services.AddHostedService(p => p.GetRequiredService<NotificationService>());

        services.AddScoped<IAuthManager, AuthManager>();


        // SignalR
        // add SignalR
        services.AddSignalR(options =>
        {
            // options.EnableDetailedErrors = true;
            options.StatefulReconnectBufferSize = 200 * 1024; // Set buffer size to 200 KB
        });

        return services;
    }


    public static WebApplication MapHubs(this WebApplication app)
    {
        app.MapHub<NotificationHub>(NotificationHub.url, options =>
        {
            options.AllowStatefulReconnects = true;
        })
            .RequireAuthorization();

        return app;
    }


}
