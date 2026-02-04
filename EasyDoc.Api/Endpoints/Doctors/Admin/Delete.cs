using EasyDoc.Api.Constants;
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Doctors.Commands.Admin;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Doctors.Admin;

internal class Delete : IEndpoint
{
    public Feature Feature => Feature.Doctors;
    public bool IsAdminEndpoint => true;
    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapDelete("/{id}", async (Guid id,
            ICommandHandler<DeleteDoctorCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteDoctorCommand(id);

            var result = await handler.HandleAsync(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);

        }).RequireAuthorization(Policies.AdminOnly);
    }
}
