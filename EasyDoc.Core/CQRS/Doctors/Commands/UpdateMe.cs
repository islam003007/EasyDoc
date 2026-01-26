using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Exceptions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Abstractions.Utils;
using EasyDoc.Application.Dtos;
using EasyDoc.Application.Errors;
using EasyDoc.Application.Services;
using EasyDoc.Domain.Constants;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Doctors.Commands;

public record UpdateMeCommand(string? PersonName,
    string? PhoneNumber,
    string? ClinicAddress,
    long? DefaultAppointmentTimeInMinutes,
    Optional<string> Description,
    Optional<string> ProfilePictureUrl) : ICommand;

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

internal class UpdateMeCommandHandler : ICommandHandler<UpdateMeCommand>
{
    private readonly IUserContext _userContext;
    private readonly DoctorsService _doctorsService;

    public UpdateMeCommandHandler(IUserContext userContext, DoctorsService doctorsService)
    {
        _userContext = userContext;
        _doctorsService = doctorsService;
    }
    public async Task<Result> HandleAsync(UpdateMeCommand command, CancellationToken cancellationToken = default)
    {
        var doctorId = _userContext.UserId;

        var updateRequest = new UpdateDoctorRequest(doctorId,
            command.PersonName,
            command.PhoneNumber,
            command.ClinicAddress,
            command.DefaultAppointmentTimeInMinutes,
            command.Description,
            command.ProfilePictureUrl);

        var result = await _doctorsService.UpdateDoctorAsync(updateRequest, cancellationToken);

        if (!result.IsSuccess && result.Error.Code == DoctorErrors.NotFoundCode)
            throw new CurrentUserNotFoundException(_userContext.UserId);

        return result;
    }
}