using EasyDoc.Api.Constants;
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Doctors.Commands.Admin;
using EasyDoc.Application.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
            IOptions<DemoOptions> demoOptions,
            CancellationToken cancellationToken) =>
        {
            if (demoOptions.Value.Enabled)
                return Results.NotFound();

            var command = new DeleteDoctorCommand(id);

            var result = await handler.HandleAsync(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);

        }).RequireAuthorization(Policies.AdminOnly);
    }
}
