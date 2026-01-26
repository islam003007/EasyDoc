using EasyDoc.Application.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyDoc.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services
        .AddOptions<ApplicationOptions>()
        .Bind(configuration.GetSection("Application"))
        .Validate(o => !string.IsNullOrWhiteSpace(o.CountryCode),
        "CountryCode must be provided")
        .ValidateOnStart();
        return services;
    }
}
