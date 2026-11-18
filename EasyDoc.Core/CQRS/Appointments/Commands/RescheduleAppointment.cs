using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Dtos;
using EasyDoc.Application.Services;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Appointments.Commands;

public record RescheduleAppointmentCommand(Guid AppointmentId, DateOnly Date, TimeOnly StartTime) : ICommand;

internal class RescheduleAppointmentCommandValidator : AbstractValidator<RescheduleAppointmentCommand>
{
    public RescheduleAppointmentCommandValidator()
    {
        RuleFor(x => x.AppointmentId)
            .NotEmpty();

        RuleFor(x => x.Date)
            .NotEmpty();

        RuleFor(x => x.StartTime)
            .NotEmpty();

        RuleFor(x => x)
            .Must(x => new DateTime(x.Date, x.StartTime, DateTimeKind.Utc) > DateTime.UtcNow)
            .WithMessage("A rescheduled appointment date and time must be in the future");
    }
}

internal class RescheduleAppointmentCommandHandler : ICommandHandler<RescheduleAppointmentCommand>
{
    private readonly AppointmentService _appointmentService;
    private readonly IUserContext _userContext;

    public RescheduleAppointmentCommandHandler(AppointmentService appointmentService, IUserContext userContext)
    {
        _appointmentService = appointmentService;
        _userContext = userContext;
    }

    public Task<Result> HandleAsync(RescheduleAppointmentCommand command, CancellationToken cancellationToken = default)
    {
        var request = new RescheduleAppointmentRequest(_userContext.PatientId,
            command.AppointmentId,
            command.Date,
            command.StartTime);

        return _appointmentService.RescheduleAppointmentAsync(request, cancellationToken);
    }
}
