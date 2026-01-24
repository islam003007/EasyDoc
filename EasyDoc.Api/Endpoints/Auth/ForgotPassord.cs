
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Auth.Commands;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Auth;

public class ForgotPassord : IEndpoint
{
    public Feature Feature => Feature.Auth;

    public bool IsAdminEndpoint => false;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/forgot-password", async (string email,
            ICommandHandler<RequestPasswordResetCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RequestPasswordResetCommand(email);

            var result = await handler.HandleAsync(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        });
    }
}
