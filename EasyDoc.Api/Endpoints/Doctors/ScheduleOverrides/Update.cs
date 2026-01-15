
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Doctors.Commands.ScheduleOverrides;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Doctors.ScheduleOverrides;

public class Update : IEndpoint
{
    public Feature Feature => Feature.Doctors;

    public bool IsAdminEndpoint => false;

    public class Request
    {
        public bool IsAvailable { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("Me/schedule-overrides/{scheduleOverrideId}", async (Request request,
            ICommandHandler<UpdateDoctorScheduleOverrideCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateDoctorScheduleOverrideCommand(request.IsAvailable, request.StartTime, request.EndTime);

            var result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);

        }).RequireAuthorization();
    }
}
