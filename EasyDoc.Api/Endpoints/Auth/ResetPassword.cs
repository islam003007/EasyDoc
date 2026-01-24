
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Auth.Commands;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Auth;

public class ResetPassword : IEndpoint
{
    public Feature Feature => Feature.Auth;

    public bool IsAdminEndpoint => false;

    public class Request
    {
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/reset-password", async (Request request,
            ICommandHandler<ResetPasswordCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ResetPasswordCommand(request.Email, request.Token, request.NewPassword);

            var result = await handler.HandleAsync(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        });
    }
}
