
using EasyDoc.Api.Constants;
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Patients.Queries.Admin;
using EasyDoc.Application.CQRS.Patients.Queries.Admin.Common;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Patients.Admin;

public class GetById : IEndpoint
{
    public Feature Feature => Feature.Patients;

    public bool IsAdminEndpoint => true;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id}", async (Guid id,
            IQueryHandler<GetPatientByIdQuery, AdminPatientResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPatientByIdQuery(id);

            var result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);

        }).RequireAuthorization(Policies.AdminOnly);
    }
}
