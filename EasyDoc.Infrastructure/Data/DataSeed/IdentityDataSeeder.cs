using EasyDoc.Application.Constants;
using EasyDoc.Infrastructure.Data.DataSeed.SeedMaterializer;
using EasyDoc.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyDoc.Infrastructure.Data.DataSeed;

internal class IdentityDataSeeder
{
    public async static Task SeedIdentityData(IServiceProvider serviceProvider)
    {
        await SeedRoles(serviceProvider);
        await SeedUsers(serviceProvider);
    }

    private async static Task SeedRoles(IServiceProvider serviceProvider)
    {
        await using (var scope = serviceProvider.CreateAsyncScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            var roles = new[]
        {
            Roles.Admin,
            Roles.Doctor,
            Roles.Patient
        };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new ApplicationRole(role));
                }
            }
        }
    }

    private async static Task SeedUsers(IServiceProvider serviceProvider)
    {
        await SeedPatientUsers(serviceProvider);
        await SeedDoctorUsers(serviceProvider);
        await SeedAdmin(serviceProvider);
    }

    private static async Task SeedAdmin(IServiceProvider serviceProvider)
    {
        await using (var scope = serviceProvider.CreateAsyncScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var adminEmail = scope.ServiceProvider.GetRequiredService<IConfiguration>()["DataSeed:AdminEmail"] ??
                throw new InvalidOperationException("DataSeed:AdminEmail is not configured");

            var adminpassword = scope.ServiceProvider.GetRequiredService<IConfiguration>()["DataSeed:AdminPassword"] ??
                throw new InvalidOperationException("DataSeed:AdminPassword is not configured");

            var existingUser = await userManager.FindByEmailAsync(adminEmail);

            if (existingUser is not null)
            {
                await userManager.AddToRoleAsync(existingUser, Roles.Admin);
                return;
            }

            var admin = new ApplicationUser(adminEmail)
            {
                EmailConfirmed = true,
            };

            var result = await userManager.CreateAsync(admin, adminpassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, Roles.Admin);
            }
        }
    }

    private async static Task SeedPatientUsers(IServiceProvider serviceProvider)
    {
        await using (var scope = serviceProvider.CreateAsyncScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var password = scope.ServiceProvider.GetRequiredService<IConfiguration>()["DataSeed:UsersPassword"] ??
                throw new InvalidOperationException("DataSeed:UsersPassword is not configured");

            var usersDataSeed = await JsonDataLoader.LoadJsonAsync<UserSeedMaterializer>("patientUsers.json");

            foreach (var user in usersDataSeed.Select(data => data.ToDomainObject()))
            {
                var existingUser = await userManager.FindByEmailAsync(user.Email!);

                if (existingUser is not null)
                {
                    await userManager.AddToRoleAsync(existingUser, Roles.Patient);
                    continue;
                }

                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, Roles.Patient);
                }
            }
        }
    }

    private async static Task SeedDoctorUsers(IServiceProvider serviceProvider)
    {
        await using (var scope = serviceProvider.CreateAsyncScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var password = scope.ServiceProvider.GetRequiredService<IConfiguration>()["DataSeed:UsersPassword"] ??
                throw new InvalidOperationException("DataSeed:UsersPassword is not configured");

            var usersDataSeed = await JsonDataLoader.LoadJsonAsync<UserSeedMaterializer>("doctorUsers.json");

            foreach (var user in usersDataSeed.Select(data => data.ToDomainObject()))
            {
                var existingUser = await userManager.FindByEmailAsync(user.Email!);

                if (existingUser is not null)
                {
                    await userManager.AddToRoleAsync(existingUser, Roles.Doctor);
                    continue;
                }

                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, Roles.Doctor);
                }
            }
        }
    }
}
