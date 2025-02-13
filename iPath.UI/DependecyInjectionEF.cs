using iPath.Data.Database;
using Microsoft.EntityFrameworkCore;
using static iPath.UI.Provider;


namespace iPath.UI;

public static class DependecyInjectionEF
{
    public static IServiceCollection AddIPathDatabase(this IServiceCollection services, IConfiguration config)
    {
        // DB connections
        //------------------------------------------------------------

        services.AddDbContext<IPathDbContext>(options =>
        {
            var provider = config.GetValue("Provider", SqlServer.Name);

            if (provider == SqlServer.Name)
            {
                options.UseSqlServer(
                    config.GetConnectionString(SqlServer.Name),
                    x => x.MigrationsAssembly(SqlServer.Assembly)
                );
            }

            if (provider == Sqlite.Name)
            {
                options.UseSqlite(
                    config.GetConnectionString(Sqlite.Name),
                    x => x.MigrationsAssembly(Sqlite.Assembly)
                );
            }

            if (provider == Postgres.Name)
            {
                options.UseNpgsql(
                    config.GetConnectionString(Postgres.Name),
                    x => x.MigrationsAssembly(Postgres.Assembly)
                );
            }
        });

        services.AddDbContextFactory<IPathDbContext>(options =>
        {
            var provider = config.GetValue("provider", SqlServer.Name);

            if (provider == SqlServer.Name)
            {
                options.UseSqlServer(
                    config.GetConnectionString(SqlServer.Name),
                    x => x.MigrationsAssembly(SqlServer.Assembly)
                );
            }

            if (provider == Sqlite.Name)
            {
                options.UseSqlite(
                    config.GetConnectionString(Sqlite.Name),
                    x => x.MigrationsAssembly(Sqlite.Assembly)
                );
            }

            if (provider == Postgres.Name)
            {
                options.UseNpgsql(
                    config.GetConnectionString(Postgres.Name),
                    x => x.MigrationsAssembly(Postgres.Assembly)
                );
            }
        }, ServiceLifetime.Scoped);
      
        return services;
    }
}


public record Provider(string Name, string Assembly)
{
    public static Provider SqlServer = new(nameof(SqlServer), typeof(iPath.Migrations.SqlServer.Marker).Assembly.GetName().Name!);
    public static Provider Sqlite = new(nameof(Sqlite), typeof(iPath.Migrations.Sqlite.Marker).Assembly.GetName().Name!);
    public static Provider Postgres = new(nameof(Postgres), typeof(iPath.Migrations.Postgres.Marker).Assembly.GetName().Name!);
}
