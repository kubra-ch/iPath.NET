using iPath.Data.Entities;
using Microsoft.EntityFrameworkCore;
using SmartEnum.EFCore;

namespace iPath.Data.Database;

public class IPathDbContext(DbContextOptions<IPathDbContext> opts) : DbContext(opts)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IPathDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ConfigureSmartEnum();
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Community> Communities { get; set; }
    public DbSet<CommunityMember> CommunityMembers { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMember> GroupMembers { get; set; }
    public DbSet<Node> Nodes { get; set; }
    public DbSet<Annotation> NodeAnnotations { get; set; }

    public DbSet<NodeField> NodeFields { get; set; }
    public DbSet<NodeXml> NodeXml { get; set; }
}
