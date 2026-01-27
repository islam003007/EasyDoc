using EasyDoc.Api.Constants;
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Doctors.Commands.Admin;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Doctors.Admin;

public class Delete : IEndpoint
{
    public Feature Feature => Feature.Doctors;
    public bool IsAdminEndpoint => true;

    public class Request
    {
        public bool IsSoftDelete { get; set; } = true;
    }

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapDelete("/{id}", async (Guid id,
            Request request,
            ICommandHandler<DeleteDoctorCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteDoctorCommand(id, request.IsSoftDelete);

            var result = await handler.HandleAsync(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);

        }).RequireAuthorization(Policies.AdminOnly);
    }
}
