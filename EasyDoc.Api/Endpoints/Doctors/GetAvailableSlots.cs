using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Doctors.Queries;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Doctors;

public class GetAvailableSlots : IEndpoint
{
    public Feature Feature => Feature.Doctors;

    public bool IsAdminEndpoint => false;

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapGet("{doctorId}/available-slots", async (Guid doctorId,
            DateOnly date,
            IQueryHandler<GetDoctorAvailableSlotsQuery, IReadOnlyList<SlotResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetDoctorAvailableSlotsQuery(doctorId, date);

            var result = await handler.HandleAsync(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        });
    }
}
