using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Patients.Commands;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Patients;

public class DeleteMe : IEndpoint
{
    public Feature Feature => Feature.Patients;

    public bool IsAdminEndpoint => false;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/me", async (ICommandHandler<DeleteMeCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteMeCommand();

            var results = await handler.HandleAsync(command, cancellationToken);

            return results.Match(Results.NoContent, CustomResults.Problem);

        }).RequireAuthorization();
    }
}
