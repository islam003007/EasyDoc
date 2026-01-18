using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Services;
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
    private readonly IUserContext _userContext;
    private readonly AppointmentService _appointmentService;

    public CancelAppointmentCommandHandler(AppointmentService appointmentService, IUserContext userContext)
    {
        _userContext = userContext;
        _appointmentService = appointmentService;
    }

    public Task<Result> Handle(CancelAppointmentCommand command, CancellationToken cancellationToken = default)
    {
        var doctorId = _userContext.DoctorId;

        return _appointmentService.CancelAppointentAsync(doctorId, command.AppointmentId, cancellationToken);
    }
}