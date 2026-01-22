using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Exceptions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Errors;
using EasyDoc.Application.Services;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Doctors.Commands.ScheduleOverrides;

public record DeleteDoctorScheduleOverrideCommand(Guid ScheduleId) : ICommand;

internal class DeleteDoctorScheduleOverrideCommandValidator : AbstractValidator<DeleteDoctorScheduleOverrideCommand>
{
    public DeleteDoctorScheduleOverrideCommandValidator()
    {
        RuleFor(x => x.ScheduleId).NotEmpty();
    }
}

internal class DeleteDoctorScheduleOverrideCommandHandler : ICommandHandler<DeleteDoctorScheduleOverrideCommand>
{
    private readonly IUserContext _userContext;
    private readonly DoctorsService _doctorsService;

    public DeleteDoctorScheduleOverrideCommandHandler(IUserContext userContext, DoctorsService doctorsService)
    {
        _userContext = userContext;
        _doctorsService = doctorsService;
    }

    public async Task<Result> HandleAsync(DeleteDoctorScheduleOverrideCommand command, CancellationToken cancellationToken = default)
    {
        var doctorId = _userContext.DoctorId;

        var result = await _doctorsService.DeleteDoctorScheduleOverrideAsync(doctorId, command.ScheduleId, cancellationToken);

        if (!result.IsSuccess && result.Error.Code == DoctorErrors.NotFoundCode)
            throw new CurrentUserNotFoundException(_userContext.UserId);

        return result;
    }
}