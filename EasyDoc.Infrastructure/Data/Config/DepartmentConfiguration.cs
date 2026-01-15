using EasyDoc.Domain.Entities.RefrenceData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyDoc.Infrastructure.Data.Config;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.Property(e => e.Name).HasMaxLength(50);
    }
}
