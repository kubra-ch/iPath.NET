using iPath.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iPath.Infrastructure.Persistance.Configuration;

internal class AnnotationConfiguration : IEntityTypeConfiguration<Annotation>
{
    public void Configure(EntityTypeBuilder<Annotation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasOne(a => a.Node).WithMany(n => n.Annotations).HasForeignKey(a => a.NodeId).IsRequired();
        builder.HasOne(a => a.Owner).WithMany().HasForeignKey(a => a.OwnerId).IsRequired().OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(a => a.CreatedOn);
        builder.HasIndex(a => a.OwnerId);
        builder.HasIndex(a => a.Visibility);
    }
}

