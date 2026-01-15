using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Infrastructure.Data;
using EasyDoc.Infrastructure.Data.Identity;
using EasyDoc.Infrastructure.Data.Interceptors;
using EasyDoc.Infrastructure.Repositories;
using EasyDoc.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyDoc.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<AddNormalizedNameAndPhoneticKeysInterceptor>();
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                   .AddInterceptors(sp.GetRequiredService<AddNormalizedNameAndPhoneticKeysInterceptor>());
        });

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IReadOnlyApplicationDbContext, ReadOnlyApplicationDbContext>();
        services.AddScoped<IUserContext, UserContext>();

        return services;
    }
}
