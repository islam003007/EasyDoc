using EasyDoc.Api.Constants;
using EasyDoc.Application.Constants;
using EasyDoc.Infrastructure.Data;
using EasyDoc.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Serilog.Core;

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

                options.Lockout.AllowedForNewUsers = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "EasyDoc.Auth";
                // default options are fine.
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

            services.AddProblemDetails(configure =>
            {
                configure.CustomizeProblemDetails = context =>
                {
                    context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
                };
            });
            return services;
        }
    }
}
