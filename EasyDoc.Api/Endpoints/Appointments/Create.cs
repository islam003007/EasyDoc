using EasyDoc.Api.Constants;
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Appointments.Commands;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Appointments;

public class Create : IEndpoint
{
    public Feature Feature => Feature.Appointments;

    public bool IsAdminEndpoint => false;

    public class Request
    {
        public Guid DoctorId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("", async (Request request,
            ICommandHandler<CreateAppointmentCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateAppointmentCommand(request.DoctorId, request.Date, request.StartTime);

            var result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);

        }).RequireAuthorization(Policies.PatientsOnly);
    }
}
