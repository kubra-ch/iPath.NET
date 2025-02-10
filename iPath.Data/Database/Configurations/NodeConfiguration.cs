using iPath.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iPath.Infrastructure.Persistance.Configuration;

internal class NodeConfiguration : IEntityTypeConfiguration<Node>
{
    public void Configure(EntityTypeBuilder<Node> builder)
    {
		builder.ToTable("Nodes");
        builder.HasKey(x => x.Id);
		builder.Property(x => x.Title).HasMaxLength(255);
		builder.Property(x => x.SubTitle).HasMaxLength(255);		
        builder.HasOne(x => x.Owner).WithMany().HasForeignKey(x => x.OwnerId).IsRequired(true);
        builder.HasMany(x => x.ChildNodes).WithOne().HasForeignKey(x => x.TopNodeId);
        builder.HasMany(x => x.Fields).WithOne(f => f.Node).HasForeignKey(x => x.NodeId);

        builder.HasOne(x => x.File).WithOne().HasForeignKey<NodeFile>(a => a.NodeId);

        builder.HasIndex(n => n.GroupId);
        builder.HasIndex(n => n.OwnerId);
        builder.HasIndex(n => n.TopNodeId);
        builder.HasIndex(n => n.CreatedOn);
    }
}


internal class NodeFileConfiguration : IEntityTypeConfiguration<NodeFile>
{
    public void Configure(EntityTypeBuilder<NodeFile> builder)
    {
        builder.Property(x => x.Filename).HasMaxLength(200).IsRequired();
        builder.Property(x => x.MimeType).HasMaxLength(100);
        builder.HasOne<Node>().WithOne(n => n.File).IsRequired().OnDelete(DeleteBehavior.Cascade);
    }
}


internal class NodeFieldConfiguration : IEntityTypeConfiguration<NodeField>
{
	public void Configure(EntityTypeBuilder<NodeField> builder)
	{
        builder.HasKey(x => x.Id);
		builder.HasIndex(f => f.Field);
        builder.HasIndex(f => f.Coding_System);
        builder.HasIndex(f => f.Coding_Code);
        builder.HasIndex(n => n.NodeId);
    }
}

internal class NodeXmlConfiguration : IEntityTypeConfiguration<NodeXml>
{
    public void Configure(EntityTypeBuilder<NodeXml> builder)
    {
        builder.HasKey(x => x.NodeId);
        builder.Property(x => x.NodeId)
            .ValueGeneratedNever();
        builder.HasIndex(n => n.NodeId);
    }
}