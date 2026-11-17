using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Services;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Appointments.Commands;

public record CancelAppointmentByPatientCommand(Guid AppointmentId) : ICommand;

internal class CancelAppointmentByPatientCommandValidator : AbstractValidator<CancelAppointmentByPatientCommand>
{
    public CancelAppointmentByPatientCommandValidator()
    {
        RuleFor(x => x.AppointmentId)
            .NotEmpty();
    }
}

internal class CancelAppointmentByPatientCommandHandler : ICommandHandler<CancelAppointmentByPatientCommand>
{
    private readonly IUserContext _userContext;
    private readonly AppointmentService _appointmentService;

    public CancelAppointmentByPatientCommandHandler(AppointmentService appointmentService, IUserContext userContext)
    {
        _userContext = userContext;
        _appointmentService = appointmentService;
    }

    public Task<Result> HandleAsync(CancelAppointmentByPatientCommand command, CancellationToken cancellationToken = default)
    {
        var patientId = _userContext.PatientId;

        return _appointmentService.CancelAppointmentByPatientAsync(patientId, command.AppointmentId, cancellationToken);
    }
}
