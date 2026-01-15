using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Doctors.Commands.ScheduleOverrides;

public record CreateDoctorScheduleOverrideCommand(DateOnly Date,
    bool IsAvailable,
    TimeOnly? StartTime,
    TimeOnly? EndTime) : ICommand<Guid>;

internal class CreateDoctorScheduleOverrideCommandValidator : AbstractValidator<CreateDoctorScheduleOverrideCommand>
{
    public CreateDoctorScheduleOverrideCommandValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty();

        RuleFor(x => x.StartTime)
            .NotEmpty().When(x => x.IsAvailable).WithMessage("Start time can only be empty if when availabilty is marked false");

        RuleFor(x => x.EndTime)
            .NotEmpty().When(x => x.IsAvailable).WithMessage("End time can only be empty if when availabilty is marked false");

        RuleFor(x => x)
            .Must(x => x.EndTime > x.StartTime).When(x => x.IsAvailable).WithMessage("End time must be after start time");

    }
}

internal class CreateDoctorScheduleOverrideCommandHandler : ICommandHandler<CreateDoctorScheduleOverrideCommand, Guid>
{
    public Task<Result<Guid>> Handle(CreateDoctorScheduleOverrideCommand command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}