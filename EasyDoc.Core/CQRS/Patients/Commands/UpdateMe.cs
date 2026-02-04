using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Abstractions.Utils;
using EasyDoc.Application.Dtos;
using EasyDoc.Application.Extensions;
using EasyDoc.Application.Services;
using EasyDoc.Domain.Constants;
using EasyDoc.SharedKernel;
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

internal class UpdateMeCommandHaneler : ICommandHandler<UpdateMeCommand>
{
    private readonly PatientService _patientService;
    private readonly IUserContext _userContext;

    public UpdateMeCommandHaneler(PatientService patientService, IUserContext userContext)
    {
        _patientService = patientService;
        _userContext = userContext;
    }

    public Task<Result> HandleAsync(UpdateMeCommand command, CancellationToken cancellationToken = default)
    {
        var request = new UpdatePatientRequest(_userContext.PatientId, command.PersonName, command.PhoneNumber);

        return _patientService.UpdatePatientAsync(request, cancellationToken);
    }
}