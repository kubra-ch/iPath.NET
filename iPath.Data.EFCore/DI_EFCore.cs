using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using static iPath.Data.EFCore.DBProvider;

namespace iPath.Data.EFCore;


public static class DI_EFCore
{
    public static IServiceCollection AddPersistance(this IServiceCollection services, IConfiguration config)
    {
        // DB connections
        //------------------------------------------------------------

        services.AddDbContext<NewDB>(options =>
        {
            var provider = config.GetValue("iPathConfig:DbProvider", SqlServer.Name);

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
                    x => { 
                        x.MigrationsAssembly(Sqlite.Assembly);
                    }
                );
            }

            if (provider == Postgres.Name)
            {
                options.UseNpgsql(
                    config.GetConnectionString(Postgres.Name),
                    x => x.MigrationsAssembly(Postgres.Assembly)
                );
            }

            if (provider == MySQL.Name)
            {
                options.UseMySQL(
                    config.GetConnectionString(MySQL.Name),
                    x => x.MigrationsAssembly(MySQL.Assembly)
                );
            }
        }, ServiceLifetime.Scoped);

        services.AddDbContextFactory<NewDB>(options =>
        {
            var provider = config.GetValue("iPathConfig:DbProvider", SqlServer.Name);

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


public record DBProvider(string Name, string Assembly)
{
    public static DBProvider SqlServer = new(nameof(SqlServer), typeof(iPath.Migrations.SqlServer.Marker).Assembly.GetName().Name!);
    public static DBProvider Sqlite = new(nameof(Sqlite), typeof(iPath.Migrations.Sqlite.Marker).Assembly.GetName().Name!);
    public static DBProvider Postgres = new(nameof(Postgres), typeof(iPath.Migrations.Postgres.Marker).Assembly.GetName().Name!);
    public static DBProvider MySQL = new(nameof(MySQL), typeof(iPath.Migrations.Mysql.Marker).Assembly.GetName().Name!);
}
