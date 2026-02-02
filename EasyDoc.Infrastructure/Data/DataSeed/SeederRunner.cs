using EasyDoc.Domain.Entities.AppointmentAggregate;
using EasyDoc.Domain.Entities.CityAggregate;
using EasyDoc.Domain.Entities.DoctorAggregate;
using EasyDoc.Domain.Entities.PatientAggregate;
using EasyDoc.Domain.Entities.RefrenceData;
using EasyDoc.Infrastructure.Data.DataSeed.SeedMaterializer;
using Microsoft.Extensions.DependencyInjection;

namespace EasyDoc.Infrastructure.Data.DataSeed;

public static class SeederRunner
{
    public static async Task SeedProduction(IServiceProvider serviceProvider)
    {

        await DataSeeder.SeedAsync<Governorate, GovernorateSeedMaterializer>(serviceProvider, "governorates.json");
        await DataSeeder.SeedAsync<City, CitySeedMaterializer>(serviceProvider, "cities.json");
        await DataSeeder.SeedAsync<Department, DepartmentSeedMaterializer>(serviceProvider, "departments.json");
    }

    public async static Task SeedDevelopment(IServiceProvider serviceProvider)
    {
        await SeedProduction(serviceProvider);
        await IdentityDataSeeder.SeedIdentityData(serviceProvider);
        await DataSeeder.SeedAsync<Doctor, DoctorSeedMaterializer>(serviceProvider, "doctors.json");
        await DataSeeder.SeedAsync<Patient, PatientSeedMaterializer>(serviceProvider, "patients.json");
        await DataSeeder.SeedAsync<Appointment, AppointmentSeedMaterializer>(serviceProvider, "appointments.json");
    }

    // Use with caution.
    public static async Task WipeDatabaseAsync(IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // 1️⃣ Delete child/dependent tables first
        context.Appointments.RemoveRange(context.Appointments);

        // 2️⃣ Delete domain tables
        context.Doctors.RemoveRange(context.Doctors);
        context.Patients.RemoveRange(context.Patients);

        context.Cities.RemoveRange(context.Cities);
        context.Governorates.RemoveRange(context.Governorates);
        context.Departments.RemoveRange(context.Departments);

        // 3️⃣ Delete Identity tables
        context.UserRoles.RemoveRange(context.UserRoles);
        context.Users.RemoveRange(context.Users);
        context.Roles.RemoveRange(context.Roles);

        // 4️⃣ Save changes
        await context.SaveChangesAsync();
    }
}
