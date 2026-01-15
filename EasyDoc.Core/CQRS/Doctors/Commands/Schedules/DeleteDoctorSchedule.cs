using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Doctors.Commands.Schedules;

public record DeleteDoctorScheduleCommand(Guid ScheduleId) : ICommand;

internal class DeleteDoctorScheduleCommandValidator : AbstractValidator<DeleteDoctorScheduleCommand>
{
    public DeleteDoctorScheduleCommandValidator()
    {
        RuleFor(x => x.ScheduleId).NotEmpty();
    }
}

internal class DeleteDoctorScheduleCommandHandler : ICommandHandler<DeleteDoctorScheduleCommand>
{
    public Task<Result> Handle(DeleteDoctorScheduleCommand command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}