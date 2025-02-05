using iPath.Data.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iPath.Data;

public static class DataDependecyInjection
{
    public static IServiceCollection AddIPathInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // DB connection
        services.AddDbContext<IPathDbContext>(options => {
            options.UseSqlServer(configuration.GetConnectionString("iPathNetConnectionString"));
        });

        return services;
    }
}
