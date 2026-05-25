using EasyDoc.Api.Constants;
using EasyDoc.Api.Endpoints;
using EasyDoc.Api.ExceptionHandlers;
using EasyDoc.Application.Constants;
using EasyDoc.Infrastructure.Data;
using EasyDoc.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EasyDoc.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWeb(this IServiceCollection services) 
        {
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

                options.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        return context.Response.WriteAsJsonAsync(new ProblemDetails
                        {
                            Status = 401,
                            Title = "Unauthorized",
                            Detail = "You must be logged in to access this resource."
                        });
                    },

                    OnRedirectToAccessDenied = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        return context.Response.WriteAsJsonAsync(new ProblemDetails
                        {
                            Status = 403,
                            Title = "Forbidden",
                            Detail = "You don't have permission to access this resource."
                        });
                    }
                };
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

            services.AddExceptionHandler<GlobalExceptionHandler>();

            services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo<IEndpoint>(), publicOnly: false)
            .AsImplementedInterfaces()
            .WithTransientLifetime());

            return services;
        }
    }
}
