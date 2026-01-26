using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Abstractions.Utils;
using EasyDoc.Domain.Constants;
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
            .MustBeValidPhoneNumber(phoneNumberService); // TODO: use libphonenumber for phone validation

    }
}
