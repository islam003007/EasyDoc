using EasyDoc.Application.Demo;
using EasyDoc.Application.Options;
using Microsoft.Extensions.Options;

namespace EasyDoc.Api.Endpoints.Auth;

internal class DemoUsers : IEndpoint
{
    public Feature Feature => Feature.Auth;
    public bool IsAdminEndpoint => false;

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapGet("/demo-users", (IOptions<DemoOptions> options) =>
        {
            if (!options.Value.Enabled)
                return Results.NotFound();

            return Results.Ok(DemoUserCatalog.All);
        });
    }
}
