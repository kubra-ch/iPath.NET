using iPath.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iPath.Infrastructure.Persistance.Configuration;

internal class AnnotationConfiguration : IEntityTypeConfiguration<Annotation>
{
    public void Configure(EntityTypeBuilder<Annotation> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
