using EasyDoc.Api.Constants;
using EasyDoc.Application.Constants;
using EasyDoc.Infrastructure.Data;
using EasyDoc.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;

namespace EasyDoc.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWeb(this IServiceCollection services) 
        {
            services.AddControllers();

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

                // the following line is to allow Lock outs so that the admin can lock accounts
                // lockoutOnFailure = false MUST BE USED TO AVOID AUTOMATIC LOCKOUTS
                options.Lockout.AllowedForNewUsers = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "EasyDoc.Auth";
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.AdminOnly, policy =>
                    policy.RequireRole(Roles.Admin));

                options.AddPolicy(Policies.DoctorsOnly, Policy =>
                    Policy.RequireRole(Roles.Doctor));

                options.AddPolicy(Policies.PatientsOnly, Policy =>
                    Policy.RequireRole(Roles.Patient));
            });
            return services;
        }
    }
}
