
using EasyDoc.Api.Constants;
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Patients.Commands.Admin;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Patients.Admin;

public class Delete : IEndpoint
{
    public Feature Feature => Feature.Patients;

    public bool IsAdminEndpoint => true;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("{id}", async (Guid id,
            ICommandHandler<DeletePatientCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeletePatientCommand(id);

            var result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);

        }).RequireAuthorization(Policies.AdminOnly);
    }
}
