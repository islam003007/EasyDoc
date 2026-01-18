using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Exceptions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Errors;
using EasyDoc.Application.Services;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Doctors.Commands.Schedules;

public record DeleteDoctorScheduleCommand(Guid ScheduleId) : ICommand;

internal class DeleteDoctorScheduleCommandValidator : AbstractValidator<DeleteDoctorScheduleCommand>
{
    public DeleteDoctorScheduleCommandValidator()
    {
        RuleFor(x => x.ScheduleId).NotEmpty();
    }
}

internal class DeleteDoctorScheduleCommandHandler : ICommandHandler<DeleteDoctorScheduleCommand>
{
    private readonly IUserContext _userContext;
    private readonly DoctorsService _doctorsService;

    public DeleteDoctorScheduleCommandHandler(IUserContext userContext, DoctorsService doctorsService)
    {
        _userContext = userContext;
        _doctorsService = doctorsService;
    }

    public async Task<Result> Handle(DeleteDoctorScheduleCommand command, CancellationToken cancellationToken = default)
    {
        var doctorId = _userContext.DoctorId;

        var result = await _doctorsService.DeleteDoctorScheduleAsync(doctorId, command.ScheduleId, cancellationToken);

        if (!result.IsSuccess && result.Error.Code == DoctorErrors.NotFoundCode)
            throw new CurrentUserNotFoundException(_userContext.UserId);

        return result;
    }
}