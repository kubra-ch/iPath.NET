using iPath.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Security;
using SmartEnum.EFCore;

namespace iPath.Data;

public class NewDB(DbContextOptions<NewDB> opts) : DbContext(opts)
{

    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<UserRefreshToken> UserRefreshToken { get; set; }
    public DbSet<Community> Communities { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Node> Nodes { get; set; }
    public DbSet<Annotation> Annotations { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ConfigureSmartEnum();
    }


    // Soft delete
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        HandleSoftDeletes();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        HandleSoftDeletes();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void HandleSoftDeletes()
    { 
        var entities = ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Deleted);
        foreach (var entity in entities)
        {
            if (entity.Entity is BaseEntityWithDeleteFlag)
            {
                entity.State = EntityState.Modified;
                var impl = entity.Entity as BaseEntityWithDeleteFlag;
                impl.DeletedOn = DateTime.UtcNow;
            }
        }
    }




    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseIdentityByDefaultColumns();


        modelBuilder.Entity<UserRole>(b =>
        {
            b.ToTable("UserRoles");
            b.HasKey(x => x.Id);

            b.Property(x => x.Name).IsRequired().HasMaxLength(100);
        });


        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("Users");

            b.Property(x => x.Username).IsRequired().HasMaxLength(200);
            b.Property(x => x.UsernameInvariant).IsRequired().HasMaxLength(200);
            b.Property(x => x.Email).IsRequired().HasMaxLength(200);
            b.Property(x => x.EmailInvariant).IsRequired().HasMaxLength(200);

            b.HasMany(x => x.GroupMembership).WithOne(m => m.User);
            b.HasMany(x => x.CommunityMembership).WithOne(m => m.User);
            b.HasMany(x => x.Roles).WithMany();

            b.OwnsOne(x => x.Profile, pb =>
            {
                pb.ToJson(); //.HasColumnType("jsonb");
                pb.OwnsMany(x => x.ContactDetails, cdb =>
                {
                    cdb.OwnsOne(cd => cd.Address);
                });
            });
            b.HasMany(x => x.Uploads).WithOne(f => f.Owner).HasForeignKey(x => x.OwnerId);

            b.HasQueryFilter(x => !x.DeletedOn.HasValue);
            b.HasIndex(x => x.DeletedOn);
        });

        modelBuilder.Entity<UserRefreshToken>(b =>
        {
            b.HasKey(x => x.Id);
        });


        modelBuilder.Entity<Community>(b =>
        {
            b.ToTable("Communities");
            b.HasKey(x => x.Id);

            b.Property(x => x.Name).HasMaxLength(200);
            b.Property(x => x.Description).HasMaxLength(500);

            b.HasOne(x => x.Owner).WithMany().HasForeignKey(x => x.OwnerId).IsRequired();

            b.HasMany(x => x.Groups).WithOne(g => g.Community).HasForeignKey(g => g.CommunityId);
            b.HasMany(x => x.Members).WithOne(m => m.Community).HasForeignKey(m => m.CommunityId).OnDelete(DeleteBehavior.NoAction);

            b.HasQueryFilter(x => !x.DeletedOn.HasValue);
            // b.HasQueryFilter(x => x.Owner == null || !x.Owner.DeletedOn.HasValue);
            b.HasIndex(x => x.DeletedOn);
        });

        modelBuilder.Entity<CommunityMember>(b =>
        {
            b.ToTable("CommunityMember");
            b.HasKey(x => x.Id);

            b.HasOne(x => x.Community).WithMany(c => c.Members).HasForeignKey(x => x.CommunityId).IsRequired();
            b.HasOne(x => x.User).WithMany(u => u.CommunityMembership).HasForeignKey(x => x.UserId).IsRequired();

            b.HasIndex(builder => new { builder.UserId, builder.CommunityId }).IsUnique();
        });


        modelBuilder.Entity<Group>(b =>
        {
            b.ToTable("Groups");
            b.HasKey(x => x.Id);

            b.Property(x => x.Name).HasMaxLength(200);

            b.HasMany(x => x.Communities).WithOne(x => x.Group).IsRequired(true);

            b.HasMany(x => x.Members).WithOne(m => m.Group).HasForeignKey(m => m.GroupId);
            b.HasMany(x => x.Nodes).WithOne(n => n.Group).HasForeignKey(n => n.GroupId).IsRequired();

            b.OwnsOne(x => x.Settings, pb =>
            {
                pb.ToJson(); //.HasColumnType("jsonb");

            });

            b.HasQueryFilter(x => !x.DeletedOn.HasValue);
            b.HasIndex(x => x.DeletedOn);
        });

        modelBuilder.Entity<CommunityGroup>(b =>
        {
            b.ToTable("CommunityGroup");
            b.HasKey(x => x.Id);

            b.HasOne(x => x.Community).WithMany(c => c.Groups).HasForeignKey(x => x.CommunityId).IsRequired();
            b.HasOne(x => x.Group).WithMany(g => g.Communities).HasForeignKey(x => x.GroupId).IsRequired();

            b.HasIndex(builder => new { builder.GroupId, builder.CommunityId }).IsUnique();
        });

        modelBuilder.Entity<GroupMember>(b =>
        {
            b.ToTable("GroupMember");
            b.HasKey(x => x.Id);

            b.HasOne(x => x.Group).WithMany(g => g.Members).HasForeignKey(x => x.GroupId).IsRequired();
            b.HasOne(x => x.User).WithMany(u => u.GroupMembership).HasForeignKey(x => x.UserId).IsRequired();

            b.HasIndex(builder => new { builder.UserId, builder.GroupId }).IsUnique();
        });




        modelBuilder.Entity<NodeImport>(b => b.HasKey(i => i.NodeId));

        modelBuilder.Entity<Node>(b =>
        {
            b.ToTable("Nodes");
            b.HasKey(x => x.Id);

            b.Property(x => x.OwnerId).IsRequired();
            b.HasOne(x => x.Owner).WithMany().HasForeignKey(x => x.OwnerId).IsRequired();
            b.HasOne(x => x.Group).WithMany(g => g.Nodes).HasForeignKey(x => x.GroupId).IsRequired(false);

            b.HasMany(x => x.ChildNodes).WithOne(c => c.RootNode).HasForeignKey(c => c.RootNodeId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(x => x.RootNode).WithMany(r => r.ChildNodes).HasForeignKey(c => c.RootNodeId).OnDelete(DeleteBehavior.NoAction);

            b.HasMany(x => x.Annotations).WithOne(a => a.Node).HasForeignKey(a => a.NodeId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(x => x.ImportedData).WithOne().OnDelete(DeleteBehavior.Cascade);
            b.HasMany(x => x.Uploads).WithOne(f => f.Node).HasForeignKey(x => x.NodeId).OnDelete(DeleteBehavior.Cascade);

            b.OwnsOne(x => x.Description, pb => pb.ToJson());
            b.OwnsOne(x => x.File, pb => pb.ToJson());

            b.HasQueryFilter(x => !x.DeletedOn.HasValue);
            b.HasIndex(x => x.DeletedOn);
        });

        modelBuilder.Entity<Annotation>(b =>
        {
            b.ToTable("Annotations");
            b.HasKey(x => x.Id);

            b.HasOne(x => x.Owner).WithMany().HasForeignKey(x => x.OwnerId).IsRequired().OnDelete(DeleteBehavior.NoAction);

            b.HasQueryFilter(x => !x.DeletedOn.HasValue);
            b.HasIndex(x => x.DeletedOn);
        });


        modelBuilder.Entity<FileUpload>(b =>
        {
            b.ToTable("FileUploads");
            b.HasKey(b => b.Id);

            b.HasOne(x => x.Node).WithMany(n => n.Uploads).IsRequired().HasForeignKey(x => x.NodeId).OnDelete(DeleteBehavior.NoAction);
            b.HasOne(x => x.Owner).WithMany(u => u.Uploads).IsRequired().HasForeignKey(x => x.OwnerId).OnDelete(DeleteBehavior.NoAction);
        });



        modelBuilder.Entity<NodeLastVisit>(b =>
        {
            b.ToTable("NodeLastVisit");
            b.HasKey(x => new { x.UserId, x.NodeId });
            b.HasIndex(x => x.Date);
            b.HasOne(x => x.User).WithMany(u => u.NodeVisitis).HasForeignKey(x => x.UserId).IsRequired().OnDelete(DeleteBehavior.NoAction);
            b.HasOne(x => x.Node).WithMany(u => u.Visits).HasForeignKey(x => x.NodeId).IsRequired().OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<UserNotification>(b =>
        {
            b.ToTable("UserNotifications");
            b.HasKey(x => x.Id);

            b.HasIndex(x => x.Date);
            b.Property(x => x.Message).HasMaxLength(500);
            b.HasOne(x => x.User).WithMany(u => u.Notifications).HasForeignKey(x => x.UserId).IsRequired().OnDelete(DeleteBehavior.NoAction);
        });


        base.OnModelCreating(modelBuilder);
    }
}
