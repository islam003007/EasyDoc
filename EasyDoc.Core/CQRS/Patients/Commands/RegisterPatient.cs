using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Abstractions.Utils;
using EasyDoc.Application.CQRS.Appointments.Commands;
using EasyDoc.Application.Dtos;
using EasyDoc.Application.Extensions;
using EasyDoc.Application.Services;
using EasyDoc.Domain.Constants;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Patients.Commands;

public record RegisterPatientCommand(string Email,
    string Password,
    string PasswordConfirm,
    string PersonName,
    string PhoneNumber) : ICommand<Guid>;

internal class RegisterPatientCommandValidator : AbstractValidator<RegisterPatientCommand>
{
    public RegisterPatientCommandValidator(IPhoneNumberService phoneNumberService)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

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
            .MustBeValidPhoneNumber(phoneNumberService);

    }
}

internal class RegisterPatientCommandHandler : ICommandHandler<RegisterPatientCommand, Guid>
{
    private readonly PatientService _patientService;

    public RegisterPatientCommandHandler(PatientService patientService)
    {
        _patientService = patientService;
    }

    public Task<Result<Guid>> HandleAsync(RegisterPatientCommand command, CancellationToken cancellationToken = default)
    {
        var request = new CreatePatientRequest(command.Email, command.Password, command.PersonName, command.PhoneNumber);

        return _patientService.CreatePatientAsync(request, cancellationToken);
    }
}