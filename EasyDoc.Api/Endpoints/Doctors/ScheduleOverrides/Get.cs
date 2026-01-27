using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Doctors.Queries.ScheduleOverrides;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Doctors.ScheduleOverrides;

internal class Get : IEndpoint
{
    public Feature Feature => Feature.Doctors;

    public bool IsAdminEndpoint => false;

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapGet("/{doctorId}/schedule-overriddes", async (Guid doctorId,
            IQueryHandler<GetDoctorScheduleOverridesQuery, IReadOnlyCollection<DoctorScheduleOverrideResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetDoctorScheduleOverridesQuery(doctorId);

            var result = await handler.HandleAsync(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        });
    }
}
