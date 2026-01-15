using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Doctors.Commands.ScheduleOverrides;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Doctors.ScheduleOverrides;

public class Delete : IEndpoint
{
    public Feature Feature => Feature.Doctors;

    public bool IsAdminEndpoint => false;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/me/schedule-overrides/{scheduleOverrideId}", async (Guid scheduleOverrideId,
            ICommandHandler<DeleteDoctorScheduleOverrideCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteDoctorScheduleOverrideCommand(scheduleOverrideId);

            var result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);

        }).RequireAuthorization();
    }
}
