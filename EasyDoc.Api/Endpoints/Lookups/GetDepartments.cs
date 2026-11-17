using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Lookups.Queries;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Lookups;

internal class GetDepartments : IEndpoint
{
    public Feature Feature => Feature.Lookups;
    public bool IsAdminEndpoint => false;

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapGet("/departments", async (IQueryHandler<GetDepartmentsQuery, IReadOnlyList<LookupResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(new GetDepartmentsQuery(), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        });
    }
}
