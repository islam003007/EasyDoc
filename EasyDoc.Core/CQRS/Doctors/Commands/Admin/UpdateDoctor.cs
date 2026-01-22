using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Dtos;
using EasyDoc.Application.Services;
using EasyDoc.Domain.Constants;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Doctors.Commands.Admin;

public record UpdateDoctorCommand(Guid DoctorId,
    string? PersonName,
    string? PhoneNumber,
    bool? isVisible,
    Guid? CityId,
    long? DefaultAppointmentTimeInMinutes,
    string? ClinicAddress,
    Optional<string> Description,
    Optional<string> ProfilePictureUrl) : ICommand;

internal class UpdateDoctorCommandValidator : AbstractValidator<UpdateDoctorCommand>
{
    public UpdateDoctorCommandValidator()
    {
        RuleFor(x => x.DoctorId)
            .NotEmpty();

        RuleFor(x => x.PersonName)
           .NotEmpty().When(x => x != null).WithMessage("Person name must not be empty if provided.")
           .MaximumLength(ProfileConstants.PersonNameMaxLength).WithMessage("Person Name should have at most 150 characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().When(x => x != null).WithMessage("Phone number must not be empty if provided.")
            .MaximumLength(PhoneNumberConstants.PhoneNumberMaxLength)
                 .WithMessage("Phone number should have at most 25 characters"); // TODO: use libphonenumber for phone validation

        RuleFor(x => x.CityId)
            .NotEmpty().When(x => x != null).WithMessage("City Id must not be empty if provided.");

        RuleFor(x => x.ClinicAddress)
            .NotEmpty().When(x => x != null).WithMessage("Clinic address Id must not be empty if provided.");

        RuleFor(x => x.DefaultAppointmentTimeInMinutes)
            .InclusiveBetween(AppointmentConstants.MinAppointmentTimeInMinutes, AppointmentConstants.MaxAppointmentTimeInMinutes);

        RuleFor(x => x.Description)
            .Must(x => x.Value == null || String.IsNullOrWhiteSpace(x.Value)).WithMessage("Description Id must not be empty if provided.");

        RuleFor(x => x.ProfilePictureUrl)
            .Must(x => x.Value == null || String.IsNullOrWhiteSpace(x.Value)).WithMessage("profile Picture Url must not be empty if provided.");

    }
}

internal class UpdateDoctorCommandHandler : ICommandHandler<UpdateDoctorCommand>
{
    private readonly DoctorsService _doctorsService;

    public UpdateDoctorCommandHandler(DoctorsService doctorsService)
    {
        _doctorsService = doctorsService;
    }
    public Task<Result> HandleAsync(UpdateDoctorCommand command, CancellationToken cancellationToken = default)
    {
        var updateRequest = new UpdateDoctorRequest(command.DoctorId,
            command.PersonName,
            command.PhoneNumber,
            command.ClinicAddress,
            command.DefaultAppointmentTimeInMinutes,
            command.Description,
            command.ProfilePictureUrl,
            command.CityId,
            command.isVisible);

        return _doctorsService.UpdateDoctorAsync(updateRequest, cancellationToken);
    }
}
