
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Doctors.Queries;
using EasyDoc.Application.CQRS.Doctors.Queries.Common;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Doctors;

internal class GetById : IEndpoint
{
    public Feature Feature => Feature.Doctors;

    public bool IsAdminEndpoint => false;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("{doctorId}", async (Guid doctorId,
            IQueryHandler<GetDoctorByIdQuery,
            DoctorResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetDoctorByIdQuery(doctorId);

            var result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        });
    }
}
