using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Doctors.Commands;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Doctors;

internal class DeleteMe : IEndpoint
{
    public Feature Feature => Feature.Doctors;

    public bool IsAdminEndpoint => false;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/Me", async (ICommandHandler<DeleteMeCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteMeCommand();

            var result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);

        }).RequireAuthorization();
    }
}
