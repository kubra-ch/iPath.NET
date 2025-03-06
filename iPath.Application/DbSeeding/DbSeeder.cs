using iPath.Application.Areas.Authentication;
using iPath.Data;
using iPath.Data.Configuration;
using iPath.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace iPath.Application.DbSeeding;

internal class DbSeeder(NewDB ctx, IOptions<iPathConfig> opts, IPasswordHasher hasher, ILogger<DbSeeder> logger)
{
    /// <summary>
    /// Initialize Database and apply Migratrions if DbAutoMigrate is configured as true
    /// </summary>
    /// <exception cref="Exception"></exception>
    public void UpdateDatabase()
    {
        if (opts.Value.DbAutoMigrate)
        {
            try
            {
                ctx.Database.EnsureCreated();
                ctx.Database.Migrate();
            }
            catch (Exception ex)
            {
                throw new Exception("No connection to database", ex);
            }
        }
    }

    /// <summary>
    /// Create Roles and initial Admin User if DbSeedingAvtice is configured as true
    /// </summary>
    public void Seed()
    {
        if( opts.Value.DbSeedingAvtice)
        {
            logger.LogInformation("seeding dataase ...");
            SeedRoles();
            SeedAdminUser();
        }
    }

    private void SeedRoles()
    {
        if( ctx.UserRoles.Count() == 0 )
        {
            logger.LogInformation("creating admin and moderator roles ...");

            ctx.UserRoles.Add(UserRole.Admin);
            ctx.UserRoles.Add(UserRole.Moderator);


            // For SqlServer enable identity insert
            if (opts.Value.DbProvider == "SqlServer")
            {
                var entityType = ctx.Model.FindEntityType(typeof(UserRole));
                var schema = entityType.GetSchema();
                string? tableName = entityType.GetTableName();

                ctx.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT dbo.{tableName} ON;");
                ctx.SaveChanges();
                ctx.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT dbo.{tableName} OFF");
            }
            else 
            {
                ctx.SaveChanges();
            }
        }
    }

    private void SeedAdminUser()
    {
        if( ctx.Users.Count() == 0)
        {
            logger.LogInformation("creating admin user ...");

            // new entity
            var admin = new User();
            admin.IsActive = true;
            admin.Username = "admin";
            admin.UsernameInvariant = "admin";
            admin.Email = "admin@test.com";
            admin.EmailInvariant = "admin@test.com";
            admin.PasswordHash = hasher.HashPassword("admin");
            admin.CreatedOn = DateTime.UtcNow;
            admin.Profile ??= new();
            admin.Profile.Initials = "A";
            admin.Profile.FirstName = "iPath";
            admin.Profile.FamilyName = "Admin";

            // assign admin role
            var adminRole = ctx.UserRoles.Find(UserRole.Admin.Id);
            admin.Roles.Add(adminRole);

            // save to DB 
            ctx.Users.Add(admin);
            ctx.SaveChanges();
        }
    }
}
