using EasyDoc.Application.Abstractions.Messaging;
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
    public Task<Result> Handle(UpdateDoctorScheduleCommand command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}