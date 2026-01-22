
using EasyDoc.Api.Constants;
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Appointments.Commands;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Appointments;

public class Accept : IEndpoint
{
    public Feature Feature => Feature.Appointments;
    public bool IsAdminEndpoint => false;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/{appointmentId}/accept", async (Guid appointmentId,
            ICommandHandler<AcceptAppointmentCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AcceptAppointmentCommand(appointmentId);

            var result = await handler.HandleAsync(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);

        }).RequireAuthorization(Policies.DoctorsOnly);
    }
}
