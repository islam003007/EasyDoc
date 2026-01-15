using EasyDoc.Api.Constants;
using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Doctors.Commands.Admin;
using EasyDoc.SharedKernel;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Doctors.Admin;

public class Update : IEndpoint
{
    public Feature Feature => Feature.Doctors;

    public bool IsAdminEndpoint => true;
    public class Request
    {
        public string? PersonName {  get; set; }
        public string? PhoneNumber { get; set; }
        public bool? isVisible { get; set; }
        public Guid? CityId { get; set; }
        public long? DefaultAppointmentTimeInMinutes { get; set; }
        public string? ClinicAddress { get; set; }
        public string? Description { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public bool? ClearDescription { get; set; }
        public bool? ClearProfilePictureUrl { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/{id}", async (Guid id,
            Request request,
            ICommandHandler<UpdateDoctorCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var optionalDescriptionResult = OptionalHelper.TryCreate(request.Description, request.ClearDescription, nameof(request.Description));

            if (!optionalDescriptionResult.IsSuccess)
                return CustomResults.Problem(optionalDescriptionResult);

            var optionalPictureResult = OptionalHelper.TryCreate(request.ProfilePictureUrl,
                request.ClearProfilePictureUrl,
                nameof(request.ProfilePictureUrl));

            if (!optionalPictureResult.IsSuccess)
                return CustomResults.Problem(optionalPictureResult);

            var command = new UpdateDoctorCommand(id,
                request.PersonName,
                request.PhoneNumber,
                request.isVisible,
                request.CityId,
                request.DefaultAppointmentTimeInMinutes,
                request.ClinicAddress,
                optionalDescriptionResult.Value,
                optionalPictureResult.Value);

            var result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);

        }).RequireAuthorization(Policies.AdminOnly);
    }
}
