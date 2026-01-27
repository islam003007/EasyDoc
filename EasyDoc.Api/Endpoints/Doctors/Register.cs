using EasyDoc.Api.Extensions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.CQRS.Doctors.Commands;
using EasyDoc.Domain.Constants;
using System.ComponentModel.DataAnnotations;
using Web.Api.Infrastructure;

namespace EasyDoc.Api.Endpoints.Doctors;

internal class Register : IEndpoint
{
    public Feature Feature => Feature.Doctors;

    public bool IsAdminEndpoint => false;

    public class Request
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string PasswordConfirm { get; set; } = null!;
        public string PersonName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string IdCardPictureUrl { get; set; } = null!;
        public string ClinicAddress { get; set; } = null!;
        public Guid DepartmentId { get; set; }
        public Guid CityId { get; set; }
        public string? Description { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public long DefaultAppointmentTimeInMinutes { get; set; } = AppointmentConstants.DefaultAppointmentTimeInMinutes;
    }
    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPost("/register", async (Request request,
            ICommandHandler<RegisterDoctorCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RegisterDoctorCommand(request.Email,
                request.Password,
                request.PasswordConfirm,
                request.PersonName,
                request.PhoneNumber,
                request.IdCardPictureUrl,
                request.ClinicAddress,
                request.DepartmentId,
                request.CityId,
                request.Description,
                request.ProfilePictureUrl,
                request.DefaultAppointmentTimeInMinutes);

            var result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        });
    }
}
