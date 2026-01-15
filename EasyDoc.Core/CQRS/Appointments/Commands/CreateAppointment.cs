using EasyDoc.Application.Abstractions.Messaging;
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
    public Task<Result<Guid>> Handle(CreateAppointmentCommand command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}