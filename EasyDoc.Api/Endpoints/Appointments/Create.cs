using EasyDoc.Api.Constants;
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Appointments.Commands;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Appointments;

internal class Create : IEndpoint
{
    public Feature Feature => Feature.Appointments;
    public bool IsAdminEndpoint => false;
    public class Request
    {
        public Guid DoctorId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
    }

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPost("", async ([FromBody]Request request,
            ICommandHandler<CreateAppointmentCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateAppointmentCommand(request.DoctorId, request.Date, request.StartTime);

            var result = await handler.HandleAsync(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);

        }).RequireAuthorization(Policies.PatientsOnly);
    }
}
