using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Exceptions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Dtos;
using EasyDoc.Application.Errors;
using EasyDoc.Application.Services;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Doctors.Commands.Schedules;

public record UpdateDoctorScheduleCommand(Guid ScheduleId, TimeOnly StartTime, TimeOnly EndTime) : ICommand;


internal class UpdateDoctorScheduleCommandValidator : AbstractValidator<UpdateDoctorScheduleCommand>
{
    public UpdateDoctorScheduleCommandValidator()
    {

        RuleFor(x => x.ScheduleId)
            .NotEmpty();

        RuleFor(x => x.StartTime)
            .NotEmpty();

        RuleFor(x => x.EndTime)
            .NotEmpty();

        RuleFor(x => x)
            .Must(x => x.StartTime < x.EndTime)
            .WithMessage("Start time must be before end time.");
    }
}

internal class UpdateDoctorScheduleCommandHandler : ICommandHandler<UpdateDoctorScheduleCommand>
{
    private readonly IUserContext _userContext;
    private readonly DoctorsService _doctorsService;

    public UpdateDoctorScheduleCommandHandler(IUserContext userContext, DoctorsService doctorsService)
    {
        _userContext = userContext;
        _doctorsService = doctorsService;
    }

    public async Task<Result> Handle(UpdateDoctorScheduleCommand command, CancellationToken cancellationToken = default)
    {
        var doctorId = _userContext.DoctorId;

        var request = new UpdateDoctorScheduleRequest(doctorId, command.ScheduleId, command.StartTime, command.EndTime);

        var result = await _doctorsService.UpdateDoctorScheduleAsync(request, cancellationToken);

        if (!result.IsSuccess && result.Error.Code == DoctorErrors.NotFoundCode)
            throw new CurrentUserNotFoundException(_userContext.UserId);

        return result;
    }
}