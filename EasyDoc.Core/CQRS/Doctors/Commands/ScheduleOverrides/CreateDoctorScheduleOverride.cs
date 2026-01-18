using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Exceptions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Dtos;
using EasyDoc.Application.Errors;
using EasyDoc.Application.Services;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Doctors.Commands.ScheduleOverrides;

public record CreateDoctorScheduleOverrideCommand(DateOnly Date,
    bool IsAvailable,
    TimeOnly? StartTime,
    TimeOnly? EndTime) : ICommand<Guid>;

internal class CreateDoctorScheduleOverrideCommandValidator : AbstractValidator<CreateDoctorScheduleOverrideCommand>
{
    public CreateDoctorScheduleOverrideCommandValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty();

        RuleFor(x => x.StartTime)
            .NotEmpty().When(x => x.IsAvailable).WithMessage("Start time can only be empty if when availabilty is marked false");

        RuleFor(x => x.EndTime)
            .NotEmpty().When(x => x.IsAvailable).WithMessage("End time can only be empty if when availabilty is marked false");

        RuleFor(x => x)
            .Must(x => x.EndTime > x.StartTime).When(x => x.IsAvailable).WithMessage("End time must be after start time");

    }
}

internal class CreateDoctorScheduleOverrideCommandHandler : ICommandHandler<CreateDoctorScheduleOverrideCommand, Guid>
{
    private readonly DoctorsService _doctorService;
    private readonly IUserContext _userContext;

    public CreateDoctorScheduleOverrideCommandHandler(DoctorsService doctorService, IUserContext userContext)
    {
        _doctorService = doctorService;
        _userContext = userContext;
    }

    public async Task<Result<Guid>> Handle(CreateDoctorScheduleOverrideCommand command, CancellationToken cancellationToken = default)
    {
        var doctorId = _userContext.DoctorId;

        var request = new CreateDoctorScheduleOverrideRequest(doctorId, command.Date, command.IsAvailable, command.StartTime, command.EndTime);

        var result = await _doctorService.CreateDoctorScheduleOverrideAsync(request, cancellationToken);

        if (!result.IsSuccess && result.Error.Code == DoctorErrors.NotFoundCode)
            throw new CurrentUserNotFoundException(_userContext.UserId); // Doctor Has to be logged in that's why that's a exceptional case

        return result;
    }
}