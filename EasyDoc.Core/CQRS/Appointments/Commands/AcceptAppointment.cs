using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Services;
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
    private readonly AppointmentService _appointmentService;
    private readonly IUserContext _userContext;

    public AcceptAppointmentCommandHandler(AppointmentService appointmentService, IUserContext userContext)
    {
        _appointmentService = appointmentService;
        _userContext = userContext;
    }

    public Task<Result> Handle(AcceptAppointmentCommand command, CancellationToken cancellationToken = default)
    {
        var doctorId = _userContext.DoctorId;

        return _appointmentService.AcceptAppointmentAsync(doctorId, command.AppointmentId, cancellationToken);
    }
}