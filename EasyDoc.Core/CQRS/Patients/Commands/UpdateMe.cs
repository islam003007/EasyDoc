using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Abstractions.Utils;
using EasyDoc.Domain.Constants;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Patients.Commands;

public record UpdateMeCommand(string? PersonName, string? PhoneNumber) : ICommand;

internal class UpdateMeCommandValidator : AbstractValidator<UpdateMeCommand>
{
    public UpdateMeCommandValidator(IPhoneNumberService phoneNumberService)
    {
        RuleFor(x => x.PersonName)
           .MaximumLength(ProfileConstants.PersonNameMaxLength);

        RuleFor(x => x.PhoneNumber)
            .MustBeValidPhoneNumber(phoneNumberService);
    }
}