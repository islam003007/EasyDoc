using EasyDoc.Domain.Entities.CityAggregate;
using EasyDoc.Domain.Entities.RefrenceData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyDoc.Infrastructure.Data.Config;

public class GovernorateConfiguration : IEntityTypeConfiguration<Governorate>
{
    public void Configure(EntityTypeBuilder<Governorate> builder)
    {
        builder.HasMany<City>().WithOne().HasForeignKey(c => c.GovernorateId).OnDelete(DeleteBehavior.Restrict);

        builder.Property(g => g.Name).HasMaxLength(100);
    }
}
