
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Auth.Commands;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Auth;

public class Login : IEndpoint
{
    public Feature Feature => Feature.Auth;

    public bool IsAdminEndpoint => false;

    public class Request
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/login", async (Request request,
            ICommandHandler<LoginCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new LoginCommand(request.Email, request.Password);

            var result = await handler.HandleAsync(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        });
    }
}
