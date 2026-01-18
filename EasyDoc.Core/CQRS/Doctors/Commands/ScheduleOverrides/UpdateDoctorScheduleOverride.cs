using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Exceptions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Dtos;
using EasyDoc.Application.Errors;
using EasyDoc.Application.Services;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Doctors.Commands.ScheduleOverrides;

public record UpdateDoctorScheduleOverrideCommand(Guid ScheduleOverrideId, bool IsAvailable, TimeOnly? StartTime, TimeOnly? EndTime) : ICommand;

internal class UpdateDoctorScheduleOverrideCommandValidator : AbstractValidator<UpdateDoctorScheduleOverrideCommand>
{
    public UpdateDoctorScheduleOverrideCommandValidator()
    {
        RuleFor(x => x.StartTime)
            .NotEmpty().When(x => x.IsAvailable).WithMessage("Start time Can't be empty when IsAvailable is marked false");

        RuleFor(x => x.EndTime)
            .NotEmpty().When(x => x.IsAvailable).WithMessage("End Time Can't be empty When IsAvailable is marked false");

        RuleFor(x => x)
            .Must(x => x.EndTime > x.StartTime).When(x => x.IsAvailable).WithMessage("End time must be after start time");
    }
}

internal class UpdateDoctorScheduleOverrideCommandHandler : ICommandHandler<UpdateDoctorScheduleOverrideCommand>
{
    private readonly IUserContext _userContext;
    private readonly DoctorsService _doctorsService;

    public UpdateDoctorScheduleOverrideCommandHandler(IUserContext userContext, DoctorsService doctorsService)
    {
        _userContext = userContext;
        _doctorsService = doctorsService;
    }

    public async Task<Result> Handle(UpdateDoctorScheduleOverrideCommand command, CancellationToken cancellationToken = default)
    {
        var doctorId = _userContext.DoctorId;

        var request = new UpdateDoctorScheduleOverrideRequest(doctorId,
            command.ScheduleOverrideId,
            command.IsAvailable,
            command.StartTime,
            command.EndTime);

        var result = await _doctorsService.UpdateDoctorScheduleOverrideAsync(request, cancellationToken);

        if (!result.IsSuccess && result.Error.Code == DoctorErrors.NotFoundCode)
            throw new CurrentUserNotFoundException(_userContext.UserId);

        return result;
    }
}