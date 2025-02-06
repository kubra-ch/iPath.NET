using iPath.Application.Configuration;
using iPath.Data.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace iPath.UI;

public static class DatabaseMigration
{

    public static async Task UpdateDatabase(this WebApplication app, IConfiguration configuration)
    {
        var opts = app.Services.GetService<IOptions<iPathConfig>>();
        if (opts != null && opts.Value.AutoMigrateDatabase)
        {
            await app.ResetDatabaseAsync(false);
        }
    }

    private static async Task ResetDatabaseAsync(this WebApplication app, bool DropDatabase)
    {
        using var scope = app.Services.CreateScope();

        var context = scope.ServiceProvider.GetService<IPathDbContext>();
        if (context != null)
        {
            if (DropDatabase)
            {
                await context.Database.EnsureDeletedAsync();
            }
            await context.Database.MigrateAsync();
        }
    }
}
