using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Auth.Queries;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Auth;

internal class Me : IEndpoint
{
    public Feature Feature => Feature.Auth;
    public bool IsAdminEndpoint => false;

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapGet("/me", async (IQueryHandler<GetMeQuery, AuthMeResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(new GetMeQuery(), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        }).RequireAuthorization();
    }
}
