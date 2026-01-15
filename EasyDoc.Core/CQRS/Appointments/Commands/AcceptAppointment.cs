using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Appointments.Commands;

public record AcceptAppointmentCommand(Guid AppointmentId) : ICommand;

internal class AcceptAppointmentCommandValidator : AbstractValidator<AcceptAppointmentCommand>
{
    public AcceptAppointmentCommandValidator()
    {
        RuleFor(x => x.AppointmentId)
            .NotEmpty();
    }
}

internal class AcceptAppointmentCommandHandler : ICommandHandler<AcceptAppointmentCommand>
{
    public Task<Result> Handle(AcceptAppointmentCommand command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}