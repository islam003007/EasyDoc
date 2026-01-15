using EasyDoc.Domain.Constants;
using EasyDoc.Domain.Entities.PatientAggregate;
using EasyDoc.Infrastructure.Data.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyDoc.Infrastructure.Data.Config;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasOne<ApplicationUser>().WithOne().HasForeignKey<Patient>(p => p.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.OwnsOne(profile => profile.PhoneNumber, phoneNumber =>
        {
            phoneNumber.Property(phoneNumber => phoneNumber.Value).HasMaxLength(25).HasColumnName("PhoneNumber");
        });
        builder.Property(p => p.PersonName).HasMaxLength(ProfileConstants.PersonNameMaxLength);
    }
}
