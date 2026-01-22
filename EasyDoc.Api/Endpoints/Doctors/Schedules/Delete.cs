
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Doctors.Commands.Schedules;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Doctors.Schedules;

public class Delete : IEndpoint
{
    public Feature Feature => Feature.Doctors;

    public bool IsAdminEndpoint => false;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/me/schedules/{scheduleId}", async (Guid scheduleId,
            ICommandHandler<DeleteDoctorScheduleCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteDoctorScheduleCommand(scheduleId);

            var result = await handler.HandleAsync(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);

        }).RequireAuthorization();
    }
}
