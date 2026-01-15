using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Doctors.Commands.ScheduleOverrides;

public record UpdateDoctorScheduleOverrideCommand(bool IsAvailable, TimeOnly? StartTime, TimeOnly? EndTime) : ICommand;

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
    public Task<Result> Handle(UpdateDoctorScheduleOverrideCommand command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}