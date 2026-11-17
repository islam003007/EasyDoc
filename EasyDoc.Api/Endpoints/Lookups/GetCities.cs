using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Lookups.Queries;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Lookups;

internal class GetCities : IEndpoint
{
    public Feature Feature => Feature.Lookups;
    public bool IsAdminEndpoint => false;

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapGet("/cities", async (IQueryHandler<GetCitiesQuery, IReadOnlyList<CityLookupResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(new GetCitiesQuery(), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        });
    }
}
