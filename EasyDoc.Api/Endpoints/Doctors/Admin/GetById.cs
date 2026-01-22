using EasyDoc.Api.Constants;
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Doctors.Queries.Admin;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Doctors.Admin;

public class GetById : IEndpoint
{
    public Feature Feature => Feature.Doctors;

    public bool IsAdminEndpoint => true;
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id}", async (Guid id, IQueryHandler<GetDoctorByIdQuery, AdminDoctorResponse> handler, CancellationToken cancellationToken) =>
        {
            var query = new GetDoctorByIdQuery(id);

            var result = await handler.HandleAsync(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);

        }).RequireAuthorization(Policies.AdminOnly);
    }
}
