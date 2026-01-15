using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Appointments.Commands;

public record CancelAppointmentCommand(Guid AppointmentId) : ICommand;

internal class CancelAppointmentCommandValidator : AbstractValidator<CancelAppointmentCommand>
{
    public CancelAppointmentCommandValidator()
    {
        RuleFor(x => x.AppointmentId)
            .NotEmpty();
    }
}

internal class CancelAppointmentCommandHandler : ICommandHandler<CancelAppointmentCommand>
{
    public Task<Result> Handle(CancelAppointmentCommand command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}