using EasyDoc.Api.Constants;
using EasyDoc.Application.Constants;

namespace EasyDoc.Api;

public static class EndPointMapper
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        using (var scope = app.ServiceProvider.CreateScope())
        {
            var endpoints = scope.ServiceProvider.GetServices<IEndpoint>();

            foreach (var featureGroup in endpoints.GroupBy(e => e.Feature))
            {
                string featureName = featureGroup.Key.ToString().ToLower();

                var standardGroup = app.MapGroup(featureName)
                                       .WithTags(featureName);

                var adminGroup = app.MapGroup($"/admin/{featureName}")
                                    .WithTags($"admin: {featureName}")
                                    .RequireAuthorization(Policies.AdminOnly); // extra check for safety

                foreach (var endpoint in featureGroup)
                {
                    var targetGroup = endpoint.IsAdminEndpoint ? adminGroup : standardGroup;

                    endpoint.MapEndpoint(targetGroup);
                }
            }

            return app;
        }
    }
}
