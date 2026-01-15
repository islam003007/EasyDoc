using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Patients.Commands;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Patients;

public class UpdateMe : IEndpoint
{
    public Feature Feature => Feature.Patients;

    public bool IsAdminEndpoint => false;

    public class Request
    {
        public string? PersonName { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/me", async (Request request,
            ICommandHandler<UpdateMeCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateMeCommand(request.PersonName, request.PhoneNumber);

            var result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);

        }).RequireAuthorization();
    }
}
