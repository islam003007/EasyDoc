using EasyDoc.Domain.Entities.AppointmentAggregate;
using EasyDoc.Domain.Entities.DoctorAggregate;
using EasyDoc.Domain.Entities.PatientAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyDoc.Infrastructure.Data.Config;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasOne(a => a.Examination).WithOne().HasForeignKey<Examination>()
            .OnDelete(DeleteBehavior.Cascade); // ef core treats this as Pk-to-PK relationship which is even better than unique indexes.
        builder.HasOne<Doctor>().WithMany().HasForeignKey(a => a.DoctorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Patient>().WithMany().HasForeignKey(a => a.PatientId).OnDelete(DeleteBehavior.Restrict);
    }
}
