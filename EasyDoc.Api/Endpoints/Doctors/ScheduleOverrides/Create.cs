using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Doctors.Commands.ScheduleOverrides;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Doctors.ScheduleOverrides;

public class Create : IEndpoint
{
    public Feature Feature => Feature.Doctors;

    public bool IsAdminEndpoint => false;

    public class Request
    {
        public DateOnly date { get; set; }
        public bool IsAvailable { get; set; }
        public TimeOnly? startTime {  get; set; }
        public TimeOnly? endTime { get; set; }
    }

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPost("/me/schedule-overrides", async (Request request,
            ICommandHandler<CreateDoctorScheduleOverrideCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateDoctorScheduleOverrideCommand(request.date, request.IsAvailable, request.startTime, request.endTime);

            var result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);

        }).RequireAuthorization();
    }
}
