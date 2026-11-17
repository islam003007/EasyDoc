using EasyDoc.Api.Constants;
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Appointments.Commands;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Appointments;

internal class CancelByPatient : IEndpoint
{
    public Feature Feature => Feature.Appointments;
    public bool IsAdminEndpoint => false;

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPost("/{appointmentId}/cancel/patient", async (Guid appointmentId,
            ICommandHandler<CancelAppointmentByPatientCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CancelAppointmentByPatientCommand(appointmentId);

            var result = await handler.HandleAsync(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        }).RequireAuthorization(Policies.PatientsOnly);
    }
}
