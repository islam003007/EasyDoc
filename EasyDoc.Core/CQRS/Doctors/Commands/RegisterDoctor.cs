using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Dtos;
using EasyDoc.Application.Services;
using EasyDoc.Domain.Constants;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Doctors.Commands;

public record RegisterDoctorCommand(string Email,
    string Password,
    string PasswordConfirm,
    string PersonName,
    string PhoneNumber,
    string IdCardPictureUrl,
    string ClinicAddress,
    Guid DepartmentId,
    Guid CityId,
    string? Description,
    string? ProfilePictureUrl,
    long DefaultAppointmentTimeInMinutes = AppointmentConstants.DefaultAppointmentTimeInMinutes) : ICommand<Guid>;

internal class RegisterDoctorCommandValidator : AbstractValidator<RegisterDoctorCommand>
{
    public RegisterDoctorCommandValidator()
    {
        // Email Validation
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        // Password Validation
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);

        RuleFor(x => x.PasswordConfirm)
            .Equal(x => x.Password);


        RuleFor(x => x.PersonName)
            .NotEmpty()
            .MaximumLength(ProfileConstants.PersonNameMaxLength);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MaximumLength(PhoneNumberConstants.PhoneNumberMaxLength); // TODO: use libphonenumber for phone validation

        RuleFor(x => x.IdCardPictureUrl)
            .NotEmpty();

        RuleFor(x => x.DepartmentId)
            .NotEmpty();

        RuleFor(x => x.CityId)
            .NotEmpty();

        RuleFor(x => x.ClinicAddress)
            .NotEmpty();

        RuleFor(x => x.DefaultAppointmentTimeInMinutes)
            .InclusiveBetween(AppointmentConstants.MinAppointmentTimeInMinutes, AppointmentConstants.MaxAppointmentTimeInMinutes);

        RuleFor(x => x.Description)
           .NotEmpty().When(x => x != null).WithMessage("Description must not be empty if provided.");

        RuleFor(x => x.ProfilePictureUrl)
           .NotEmpty().When(x => x != null).WithMessage("Profile picture url must not be empty if provided.");

    }
}

internal class RegisterDoctorCommandHandler : ICommandHandler<RegisterDoctorCommand, Guid>
{
    private readonly DoctorsService _doctorService;

    public RegisterDoctorCommandHandler(DoctorsService doctorService)
    {
        _doctorService = doctorService;
    }

    public Task<Result<Guid>> Handle(RegisterDoctorCommand command, CancellationToken cancellationToken = default)
    {
        var request = new CreateDoctorRequest(command.Email,
            command.Password,
            command.PersonName,
            command.PhoneNumber,
            command.IdCardPictureUrl,
            command.DepartmentId,
            command.CityId,
            command.ClinicAddress,
            command.Description,
            command.ProfilePictureUrl,
            command.DefaultAppointmentTimeInMinutes);

        return _doctorService.CreateDoctorAsync(request, cancellationToken);
    }
}