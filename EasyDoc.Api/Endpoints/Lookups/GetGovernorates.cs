using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Lookups.Queries;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Lookups;

internal class GetGovernorates : IEndpoint
{
    public Feature Feature => Feature.Lookups;
    public bool IsAdminEndpoint => false;

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapGet("/governorates", async (IQueryHandler<GetGovernoratesQuery, IReadOnlyList<GovernorateLookupResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(new GetGovernoratesQuery(), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        });
    }
}
