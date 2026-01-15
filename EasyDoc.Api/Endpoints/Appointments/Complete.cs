
using EasyDoc.Api.Constants;
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Appointments.Commands;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Appointments;

public class Complete : IEndpoint
{
    public Feature Feature => Feature.Appointments;

    public bool IsAdminEndpoint => false;

    public class Request
    {
        public string Diagnosis { get; set; } = null!;
        public string Prescription { get; set; } = null!;
        public string? Notes { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/{appointmentId}/Complete", async (Guid appointmentId,
            Request request,
            ICommandHandler<CompleteAppointmentCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CompleteAppointmentCommand(appointmentId, request.Diagnosis, request.Prescription, request.Notes);

            var result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);

        }).RequireAuthorization(Policies.DoctorsOnly);
    }
}
