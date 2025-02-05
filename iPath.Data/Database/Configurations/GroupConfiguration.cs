using iPath.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace iPath.Infrastructure.Persistance.Configuration;

internal class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).HasMaxLength(200);

        builder.HasOne(builder => builder.Community).WithMany(c => c.Groups).HasForeignKey(x => x.CommunityId).IsRequired(false);
        builder.HasIndex(g => g.CommunityId);

        builder.HasOne(d => d.Owner).WithMany().HasForeignKey(d => d.OwnerId).IsRequired(false);

        builder.HasMany(g => g.Members).WithOne().HasForeignKey(m => m.GroupId).OnDelete(DeleteBehavior.NoAction);
        builder.HasMany(g => g.Nodes).WithOne(n => n.Group);

        builder.OwnsOne(g => g.Settings, sbuilder => 
        {
            sbuilder.ToJson();
        });
    }
}


internal class GroupMemberConfiguration : IEntityTypeConfiguration<GroupMember>
{
    public void Configure(EntityTypeBuilder<GroupMember> builder)
    {
        builder.HasKey(m => m.Id);

        /*
        builder.HasOne(m => m.User).WithMany().HasForeignKey(m => m.UserId).IsRequired();
        builder.HasOne(m => m.Group).WithMany(g => g.Members).HasForeignKey(m => m.GroupId).IsRequired();
        */ 

        builder.HasIndex(builder => new { builder.UserId, builder.GroupId }).IsUnique();
        builder.HasIndex(g => g.UserId);
    }
}