using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Exceptions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Dtos;
using EasyDoc.Application.Errors;
using EasyDoc.Application.Services;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Doctors.Commands.Schedules;

public record CreateDoctorScheduleCommand(DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime) : ICommand<Guid>;

internal class CreateDoctorScheduleCommandValidator : AbstractValidator<CreateDoctorScheduleCommand>
{
    public CreateDoctorScheduleCommandValidator()
    {

        RuleFor(x => x.DayOfWeek)
            .IsInEnum();

        RuleFor(x => x.StartTime)
            .NotEmpty();

        RuleFor(x => x.EndTime)
            .NotEmpty();

        RuleFor(x => x)
            .Must(x => x.EndTime > x.StartTime)
            .WithMessage("End time must be after start time");

    }
}

internal class CreateDoctorScheduleCommandHandler : ICommandHandler<CreateDoctorScheduleCommand, Guid>
{
    private readonly DoctorsService _doctorService;
    private readonly IUserContext _userContext;
    public CreateDoctorScheduleCommandHandler(DoctorsService doctorService, IUserContext userContext)
    {
        _doctorService = doctorService;
        _userContext = userContext;
    }
    public async Task<Result<Guid>> Handle(CreateDoctorScheduleCommand command, CancellationToken cancellationToken = default)
    {
        var doctorId = _userContext.DoctorId;

        var request = new CreateDoctorScheduleRequest(doctorId, command.DayOfWeek, command.StartTime, command.EndTime);

        var result = await _doctorService.CreateDoctorScheduleAsync(request, cancellationToken);

        if (!result.IsSuccess && result.Error.Code == DoctorErrors.NotFoundCode)
            throw new CurrentUserNotFoundException(_userContext.UserId); // Doctor Has to be logged in that's why that's a exceptional case

        return result;
    }
}


