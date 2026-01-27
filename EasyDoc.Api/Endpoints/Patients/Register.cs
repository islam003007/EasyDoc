using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Patients.Commands;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Patients;

public class Register : IEndpoint
{
    public Feature Feature => Feature.Patients;

    public bool IsAdminEndpoint => false;

    public class Request
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string PasswordConfirm { get; set; } = null!;
        public string PersonName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPost("/register", async (Request request,
            ICommandHandler<RegisterPatientCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RegisterPatientCommand(request.Email,
                request.Password,
                request.PasswordConfirm,
                request.PersonName,
                request.PhoneNumber);

            var result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        });
    }
}
