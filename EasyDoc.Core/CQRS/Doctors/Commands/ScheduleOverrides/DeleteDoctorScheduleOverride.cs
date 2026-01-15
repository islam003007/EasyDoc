using EasyDoc.Application.Abstractions.Messaging;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Doctors.Commands.ScheduleOverrides;

public record DeleteDoctorScheduleOverrideCommand(Guid ScheduleId) : ICommand;

internal class DeleteDoctorScheduleOverrideCommandValidator : AbstractValidator<DeleteDoctorScheduleOverrideCommand>
{
    public DeleteDoctorScheduleOverrideCommandValidator()
    {
        RuleFor(x => x.ScheduleId).NotEmpty();
    }
}