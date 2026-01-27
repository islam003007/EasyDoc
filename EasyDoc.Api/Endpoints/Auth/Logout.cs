using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Auth.Commands;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Auth;

public class Logout : IEndpoint
{
    public Feature Feature => Feature.Auth;

    public bool IsAdminEndpoint => false;

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPost("/logout", async (ICommandHandler<LogoutCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new LogoutCommand();

            var result = await handler.HandleAsync(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        });
    }
}
