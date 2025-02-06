using iPath.Application.Configuration;
using iPath.Application.Features;
using iPath.Application.Services;
using iPath.Data;
using iPath.Data.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
