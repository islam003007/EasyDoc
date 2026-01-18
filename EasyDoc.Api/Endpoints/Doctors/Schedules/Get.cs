
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Doctors.Queries.Schedules;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Doctors.Schedules;

internal class Get : IEndpoint
{
    public Feature Feature => Feature.Doctors;

    public bool IsAdminEndpoint => false;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/{doctorId}/schedules", async (Guid doctorId,
            IQueryHandler<GetDoctorSchedulesQuery,
            IReadOnlyList<DoctorScheduleResponse>> handler,
           CancellationToken cancellationToken) =>
        {
            var query = new GetDoctorSchedulesQuery(doctorId);

            var result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        });
    }
}
