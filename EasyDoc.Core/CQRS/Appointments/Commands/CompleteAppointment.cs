using EasyDoc.Application.Abstractions.Messaging;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Appointments.Commands;

public record CompleteAppointmentCommand(Guid AppointmentId, string Diagnosis, string Prescription, string? Notes) : ICommand;

internal class CompleteAppointmentCommandValidator : AbstractValidator<CompleteAppointmentCommand>
{
    public CompleteAppointmentCommandValidator()
    {
        RuleFor(x => x.AppointmentId)
            .NotEmpty();

        RuleFor(x => x.Diagnosis)
            .NotEmpty();

        RuleFor(x => x.Prescription)
            .NotEmpty();

        RuleFor(x => x.Notes)
            .NotEmpty().When(x => x.Notes is not null).WithMessage("Notes can't be an empty if it was provided.");
    }
}