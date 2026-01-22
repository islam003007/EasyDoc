using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Patients.Queries;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Patients;

public class GetMe : IEndpoint
{
    public Feature Feature => Feature.Patients;

    public bool IsAdminEndpoint => false;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/me", async (IQueryHandler<GetMeQuery, PatientMeResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetMeQuery();

            var result = await handler.HandleAsync(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);

        }).RequireAuthorization();
    }
}
