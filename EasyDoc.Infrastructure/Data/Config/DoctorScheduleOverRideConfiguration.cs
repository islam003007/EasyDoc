using EasyDoc.Domain.Entities.DoctorAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace EasyDoc.Infrastructure.Data.Config;

public class DoctorScheduleOverRideConfiguration : IEntityTypeConfiguration<DoctorScheduleOverride>
{
    public void Configure(EntityTypeBuilder<DoctorScheduleOverride> builder)
    {
        builder.HasIndex("DoctorId", "Date").IsUnique();
    }
}
