using EasyDoc.Application.Constants;
using EasyDoc.Domain.Entities.CityAggregate;
using EasyDoc.Domain.Entities.RefrenceData;
using EasyDoc.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EasyDoc.Infrastructure.Data;

public static class ApplicationDbContextSeed // TODO: error handling and a transaction
{
    public static async Task Seed(ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        await SeedIdentity(dbContext, userManager, roleManager);
        await SeedGovenNorates(dbContext);
        await SeedCities(dbContext);
        await SeedDepartments(dbContext);
    }
    
    private static async Task SeedIdentity(ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        dbContext.Database.Migrate();

        await roleManager.CreateAsync(new ApplicationRole(Roles.Admin));
        await roleManager.CreateAsync(new ApplicationRole(Roles.Doctor));
        await roleManager.CreateAsync(new ApplicationRole(Roles.Patient));
    }

    private static async Task SeedGovenNorates(ApplicationDbContext dbContext)
    {
        if (await dbContext.Governorates.AnyAsync()) return;
        IEnumerable<Governorate> governorates = new List<Governorate>
        {
            new("Cairo"),
            new("Giza"),
            new("Alexandria"),
            new("Qalyubia"),
            new("Dakahlia"),
            new("Sharqia"),
            new("Beheira"),
            new("Gharbia"),
            new("Monufia"),
            new("Kafr El Sheikh"),
            new("Fayoum"),
            new("Beni Suef"),
            new("Minya"),
            new("Asyut"),
            new("Sohag"),
            new("Qena"),
            new("Luxor"),
            new("Aswan"),
            new("Red Sea"),
            new("New Valley"),
            new("Matrouh"),
            new("North Sinai"),
            new("South Sinai"),
            new("Port Said"),
            new("Ismailia"),
            new("Suez"),
            new("Damietta")
        };
        await dbContext.Governorates.AddRangeAsync(governorates);
        await dbContext.SaveChangesAsync();

    }

    private static async Task SeedCities(ApplicationDbContext dbContext) // Only sohag cities right now
    {
        if (await dbContext.Cities.AnyAsync()) return;
        Guid sohagGovenorateId = await dbContext.Governorates.Where(g => g.Name == "Sohag").Select(g => g.Id).FirstAsync();
        IEnumerable<City> cities = new List<City>
        {
            new City("Sohag", sohagGovenorateId),
            new City("Girga", sohagGovenorateId),
            new City("Saqultah", sohagGovenorateId),
            new City("Akhmim", sohagGovenorateId),
            new City("Tahta", sohagGovenorateId),
            new City("Dar El Salam", sohagGovenorateId),
        };

        await dbContext.Cities.AddRangeAsync(cities);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedDepartments(ApplicationDbContext dbContext)
    {
        if (await dbContext.Departments.AnyAsync()) return;

        IEnumerable<Department> departments = new List<Department>
        {
            new Department("Orthopedics"),
            new Department("Cardiology"),
            new Department("Surgery"),
            new Department("Ophthalmology"),
            new Department("Pediatrics"),
            new Department("Dentistry"),
        };

        await dbContext.Departments.AddRangeAsync(departments);
        await dbContext.SaveChangesAsync();
    }
}
