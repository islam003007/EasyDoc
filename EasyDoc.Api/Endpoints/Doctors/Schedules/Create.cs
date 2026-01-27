using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Doctors.Commands.Schedules;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Doctors.Schedules;

public class Create : IEndpoint
{
    public Feature Feature => Feature.Doctors;

    public bool IsAdminEndpoint => false;

    public class Request
    {
        public DayOfWeek dayOfWeek {  get; set; }
        public TimeOnly startTime {  get; set; }
        public TimeOnly endTime { get; set; }
    }

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPost("/me/schedules", async (Request request,
            ICommandHandler<CreateDoctorScheduleCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateDoctorScheduleCommand(request.dayOfWeek, request.startTime, request.endTime);

            var result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);

        }).RequireAuthorization();
    }
}
