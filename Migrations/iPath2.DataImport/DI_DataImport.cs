using iPath.Data.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iPath2.DataImport;


public static class DI_DataImport
{
    public static IServiceCollection AddIPath2DataImport(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<OldDB>(options =>
        {
            options.UseMySQL(config.GetConnectionString("ipath_old"));
            // options.LogTo(Console.WriteLine);
        });

        services.AddDbContextFactory<OldDB>(options =>
        {
            options.UseMySQL(config.GetConnectionString("ipath_old"));
            // options.LogTo(Console.WriteLine);
        }, ServiceLifetime.Scoped);

        services.Configure<iPathConfig>(options => config.GetSection(nameof(iPathConfig)).Bind(options));
        services.AddScoped<IImportService, ImportService>();

        return services;
    }
}
