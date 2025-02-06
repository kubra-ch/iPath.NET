using iPath.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iPath.Infrastructure.Persistance.Configuration;

internal class CommunityConfiguration : IEntityTypeConfiguration<Community>
{
    public void Configure(EntityTypeBuilder<Community> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).HasMaxLength(200);

        builder.HasOne(c => c.Owner).WithMany().HasForeignKey(c => c.OwnerId).IsRequired();

        builder.HasMany(c => c.Members).WithOne().HasForeignKey(x => x.CommunityId).OnDelete(DeleteBehavior.NoAction);
        builder.HasMany(c => c.Groups).WithOne().HasForeignKey(x => x.CommunityId).OnDelete(DeleteBehavior.NoAction);

        builder.OwnsOne(c => c.Settings, sbuilder => 
        {
            sbuilder.ToJson();
        });
    }
}



internal class CommunityMemberConfiguration : IEntityTypeConfiguration<CommunityMember>
{
    public void Configure(EntityTypeBuilder<CommunityMember> builder)
    {
        builder.HasKey(m => m.Id);
        builder.HasOne(m => m.User).WithMany().HasForeignKey(m => m.UserId).IsRequired();
        builder.HasOne(m => m.Community).WithMany(c => c.Members).HasForeignKey(m => m.CommunityId).IsRequired();
        builder.HasIndex(builder => new { builder.UserId, builder.CommunityId }).IsUnique();
        builder.HasIndex(m => m.CommunityId);
    }
}