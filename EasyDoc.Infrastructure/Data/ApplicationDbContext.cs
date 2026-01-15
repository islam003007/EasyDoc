using EasyDoc.Application.CQRS.Doctors.Queries.Common;
using EasyDoc.Domain.Entities.AppointmentAggregate;
using EasyDoc.Domain.Entities.CityAggregate;
using EasyDoc.Domain.Entities.DoctorAggregate;
using EasyDoc.Domain.Entities.PatientAggregate;
using EasyDoc.Domain.Entities.RefrenceData;
using EasyDoc.Infrastructure.Data.Identity;
using EasyDoc.Infrastructure.Data.Interceptors;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Reflection.Emit;
namespace EasyDoc.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public ApplicationDbContext():base() { }
    public ApplicationDbContext(DbContextOptions options) : base(options) { }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Governorate> Governorates { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<DoctorDetailsReadModel> DoctorDetails { get; set; }

    public ApplicationDbContext(DbContextOptions options,
        AddNormalizedNameAndPhoneticKeysInterceptor interceptor): base(options) 
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ConfigureIdentity();
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Entity<DoctorDetailsReadModel>()
        .HasNoKey()
        .ToView("vw_DoctorDetails", "dbo");
    }
}
