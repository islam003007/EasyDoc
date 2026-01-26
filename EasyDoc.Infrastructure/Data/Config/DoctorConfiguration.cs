using EasyDoc.Domain.Constants;
using EasyDoc.Domain.Entities.CityAggregate;
using EasyDoc.Domain.Entities.DoctorAggregate;
using EasyDoc.Domain.Entities.RefrenceData;
using EasyDoc.Infrastructure.Data.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Net.NetworkInformation;

namespace EasyDoc.Infrastructure.Data.Config;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.HasOne<Department>().WithMany().HasForeignKey(doctor => doctor.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<City>().WithMany().HasForeignKey(d => d.CityId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(d => d.Schedules).WithOne(schedule => schedule.Doctor).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(d => d.ScheduleOverrides).WithOne(schedule => schedule.Doctor).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<ApplicationUser>().WithOne().HasForeignKey<Doctor>(d => d.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.OwnsOne(profile => profile.PhoneNumber, phoneNumber =>
        {
            phoneNumber.Property(phoneNumber => phoneNumber.Value)
            .HasMaxLength(25).HasColumnName("PhoneNumber");
        });

        builder.Property(d => d.IdCardPictureUrl).HasMaxLength(1500);
        builder.Property(d => d.ClinicAddress).HasMaxLength(500);
        builder.Property(d => d.Description).HasMaxLength(2000);
        builder.Property(d => d.ProfilePictureUrl).HasMaxLength(1500);
        builder.Property(d => d.PersonName).HasMaxLength(ProfileConstants.PersonNameMaxLength);
        builder.Property(d => d.NormalizedName).HasMaxLength(ProfileConstants.MetaphoneKeysMaxLength);

        builder.HasIndex(d => d.UserId);
        builder.HasIndex(d => new { d.CityId, d.DepartmentId })
               .IncludeProperties(d => new { d.Id, d.PersonName, d.IdCardPictureUrl })
               .HasFilter("[IsVisible] = 1"); // This index is to improve querying for all doctors can add another key for sorting
                                              // to make it even more performant.

        builder.HasIndex(d => d.CityId);
        builder.HasIndex(d => d.DepartmentId); // these 2 are explicit because ef core drops them when creating the composite index
                                               // as they are still needed for admin queries
    }
}
