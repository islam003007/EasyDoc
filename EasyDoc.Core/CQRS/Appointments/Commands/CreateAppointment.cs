using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Exceptions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Dtos;
using EasyDoc.Application.Errors;
using EasyDoc.Application.Services;
using EasyDoc.Domain.Constants;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Appointments.Commands;

public record CreateAppointmentCommand(Guid DoctorId, DateOnly Date, TimeOnly StartTime) : ICommand<Guid>;

internal class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {

        RuleFor(x => x.DoctorId)
            .NotEmpty();

        RuleFor(x => new DateTime(x.Date, x.StartTime, DateTimeKind.Utc))
            .Must(x => x > DateTime.UtcNow).WithMessage("An appointment date and time mustn't be in the past")
            .Must(x => x < DateTime.UtcNow.AddDays(AppointmentConstants.MaxAppointmentLeadTimeInDays))
                .WithMessage($"An Appointment should be at max {AppointmentConstants.MaxAppointmentLeadTimeInDays} days early in advance");
    }
}

internal class CreateAppointmentCommandHandler : ICommandHandler<CreateAppointmentCommand, Guid>
{
    private readonly AppointmentService _appointmentService;
    private readonly IUserContext _userContext;

    public CreateAppointmentCommandHandler(AppointmentService appointmentService, IUserContext userContext)
    {
        _appointmentService = appointmentService;
        _userContext = userContext;
    }

    public async Task<Result<Guid>> Handle(CreateAppointmentCommand command, CancellationToken cancellationToken = default)
    {
        var patientId = _userContext.PatientId;

        var request = new CreateAppointmentRequest(patientId, command.DoctorId, command.Date, command.StartTime);

        var result = await _appointmentService.CreateAppointmentAsync(request, cancellationToken);

        if (!result.IsSuccess && result.Error.Code == PatientErrors.NotFoundCode)
            throw new CurrentUserNotFoundException(_userContext.UserId);

        return result;
    }
}