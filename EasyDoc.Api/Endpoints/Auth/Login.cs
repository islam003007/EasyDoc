using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Auth.Commands;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Auth;

internal class Login : IEndpoint
{
    public Feature Feature => Feature.Auth;
    public bool IsAdminEndpoint => false;
    public class Request
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPost("/login", async ([FromBody] Request request,
            ICommandHandler<LoginCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new LoginCommand(request.Email, request.Password);

            var result = await handler.HandleAsync(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        });
    }
}
