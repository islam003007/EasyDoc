using EasyDoc.Api.Constants;
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Appointments.Commands;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Appointments;

internal class Reschedule : IEndpoint
{
    public Feature Feature => Feature.Appointments;
    public bool IsAdminEndpoint => false;

    public class Request
    {
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
    }

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPatch("/{appointmentId}/reschedule", async (Guid appointmentId,
            [FromBody] Request request,
            ICommandHandler<RescheduleAppointmentCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RescheduleAppointmentCommand(appointmentId, request.Date, request.StartTime);

            var result = await handler.HandleAsync(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        }).RequireAuthorization(Policies.PatientsOnly);
    }
}
