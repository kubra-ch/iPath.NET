using iPath.Application.Features;
using iPath.Application.Services;
using iPath.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iPath.Application;

public static class ApplicationDependecyInjection
{
    public static IServiceCollection AddIPathApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIPathInfrastructure(configuration);

        services.AddMediatR(opts => opts.RegisterServicesFromAssemblyContaining<GetUserQuery>());
        services.AddTransient<IPasswordHasher, PasswordHasher>();
        services.AddTransient<ThumbImageService, ThumbImageService>();

        return services;
    }
}
