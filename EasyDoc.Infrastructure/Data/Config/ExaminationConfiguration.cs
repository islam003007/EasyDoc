using EasyDoc.Domain.Entities.AppointmentAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyDoc.Infrastructure.Data.Config
{
    public class ExaminationConfiguration : IEntityTypeConfiguration<Examination>
    {
        public void Configure(EntityTypeBuilder<Examination> builder)
        {
            builder.Property(e => e.Notes).HasMaxLength(2000);
            builder.Property(e => e.Diagnosis).HasMaxLength(2000);
            builder.Property(e => e.Prescription).HasMaxLength(2000);
        }
    }
}
