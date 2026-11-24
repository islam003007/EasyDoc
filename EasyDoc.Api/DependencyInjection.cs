using EasyDoc.Api.Constants;
using EasyDoc.Api.Endpoints;
using EasyDoc.Api.ExceptionHandlers;
using EasyDoc.Api.Options;
using EasyDoc.Application.Constants;
using EasyDoc.Application.Options;
using EasyDoc.Infrastructure.Data;
using EasyDoc.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.RateLimiting;

namespace EasyDoc.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWeb(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DemoOptions>(configuration.GetSection("Demo"));

            var allowedOrigins = configuration
                .GetSection("Frontend:AllowedOrigins")
                .Get<string[]>() ?? [];

            services.AddCors(options => options.AddPolicy(Policies.FrontendCors, policy =>
            {
                if (allowedOrigins.Length == 0)
                {
                    return;
                }

                policy.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            }));

            var rateLimit = configuration
                .GetSection(RateLimitOptions.SectionName)
                .Get<RateLimitOptions>() ?? new RateLimitOptions();

            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    if (!rateLimit.Enabled)
                    {
                        return RateLimitPartition.GetNoLimiter("disabled");
                    }

                    var partitionKey = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ =>
                        new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = rateLimit.PermitLimit,
                            Window = TimeSpan.FromSeconds(rateLimit.WindowSeconds),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });
            });

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
                    context.ProblemDetails.Extensions.TryAdd("RequestId", context.HttpContext.TraceIdentifier);
                    context.ProblemDetails.Extensions.TryAdd("TraceId", Activity.Current?.TraceId);
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
