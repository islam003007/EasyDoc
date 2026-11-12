using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Infrastructure.Data;
using EasyDoc.Infrastructure.Data.Interceptors;
using EasyDoc.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EasyDoc.Application.Abstractions.Utils;
using EasyDoc.Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using EasyDoc.Infrastructure.Data.Identity;

namespace EasyDoc.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                   .AddInterceptors(sp.GetRequiredService<AddNormalizedNameAndPhoneticKeysInterceptor>());
        });

        services.AddSingleton<AddNormalizedNameAndPhoneticKeysInterceptor>();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IReadOnlyApplicationDbContext, ReadOnlyApplicationDbContext>();

        services.AddScoped<IUserContext, UserContext>();

        services.AddSingleton<IPhoneNumberService, PhoneNumberService>();

        services.AddScoped<IDoctorSearchService, DoctorSearchService>();

        services.AddScoped<IEmailSender, LogEmailSender>();
        services.AddScoped<IEmailSender<ApplicationUser>, IdentityEmailSender>();

        services.AddScoped<ISignInService, SignInService>();

        services.AddScoped<IUserService, UserService>();

        services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();

        services.AddScoped<UserNotificationService>();

        return services;
    }
}
