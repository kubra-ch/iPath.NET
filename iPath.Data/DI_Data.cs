using iPath.Data.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iPath.Data;

public static class DI_Data
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<iPathConfig>(options => config.GetSection(nameof(iPathConfig)).Bind(options));

        return services;
    }
}

