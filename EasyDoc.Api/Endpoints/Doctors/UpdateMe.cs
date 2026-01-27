using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Doctors.Commands;
using EasyDoc.SharedKernel;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Doctors;

internal class UpdateMe : IEndpoint
{
    public Feature Feature => Feature.Doctors;

    public bool IsAdminEndpoint => false;

    public class Request
    {
        public string? PersonName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ClinicAddress { get; set; }
        public long? DefaultAppointmentTimeInMinutes { get; set; }
        public string? Description { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public bool? ClearDescription { get; set; } // simply updating them to null
        public bool? ClearProfilePictureUrl { get; set; }
    }

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPatch("/Me", async (Request request,
            ICommandHandler<UpdateMeCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var descriptionResult = OptionalHelper.TryCreate(request.Description, request.ClearDescription, nameof(request.Description));

            if (!descriptionResult.IsSuccess)
                return CustomResults.Problem(descriptionResult);

            var picResult = OptionalHelper.TryCreate(request.ProfilePictureUrl, request.ClearProfilePictureUrl, nameof(request.ProfilePictureUrl));

            if (!picResult.IsSuccess)
                return CustomResults.Problem(picResult);

            var command = new UpdateMeCommand(request.PersonName,
                request.PhoneNumber,
                request.ClinicAddress,
                request.DefaultAppointmentTimeInMinutes,
                descriptionResult.Value,
                picResult.Value
                );

            var result = await handler.HandleAsync(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        }).RequireAuthorization();
    }
}
