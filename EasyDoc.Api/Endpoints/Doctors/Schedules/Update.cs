using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Doctors.Commands.Schedules;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Doctors.Schedules;

public class Update : IEndpoint
{
    public Feature Feature => Feature.Doctors;

    public bool IsAdminEndpoint => false;

    public class Request
    {
        public TimeOnly startTime {  get; set; }
        public TimeOnly endTime { get; set; }
    }

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPatch("/me/schedules/{scheduleId}", async (Guid scheduleId,
            Request request,
            ICommandHandler<UpdateDoctorScheduleCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateDoctorScheduleCommand(scheduleId, request.startTime, request.endTime);

            var result = await handler.HandleAsync(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);

        }).RequireAuthorization();
    }
}
