using EasyDoc.Api.Constants;
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Doctors.Commands.ScheduleOverrides;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Doctors.ScheduleOverrides;

internal class Update : IEndpoint
{
    public Feature Feature => Feature.Doctors;
    public bool IsAdminEndpoint => false;
    public class Request
    {
        public bool IsAvailable { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
    }

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPatch("Me/schedule-overrides/{scheduleOverrideId}", async (Guid scheduleOverrideId,
            [FromBody] Request request,
            ICommandHandler<UpdateDoctorScheduleOverrideCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateDoctorScheduleOverrideCommand(scheduleOverrideId, request.IsAvailable, request.StartTime, request.EndTime);

            var result = await handler.HandleAsync(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);

        }).RequireAuthorization(Policies.DoctorsOnly);
    }
}
