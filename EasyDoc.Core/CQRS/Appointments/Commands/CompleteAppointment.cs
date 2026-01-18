using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Services;
using EasyDoc.SharedKernel;
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

internal class CompleteAppointmentCommandHandler : ICommandHandler<CompleteAppointmentCommand>
{
    private readonly IUserContext _userContext;
    private readonly AppointmentService _appointmentService;

    public CompleteAppointmentCommandHandler(IUserContext userContext, AppointmentService appointmentService)
    {
        _userContext = userContext;
        _appointmentService = appointmentService;
    }

    public Task<Result> Handle(CompleteAppointmentCommand command, CancellationToken cancellationToken = default)
    {
        var doctorId = _userContext.DoctorId;

        var request = new CompleteAppointmentRequest(doctorId, command.AppointmentId, command.Diagnosis, command.Prescription, command.Notes);

        return _appointmentService.CompleteAppointmentAsync(request, cancellationToken);
    }
}