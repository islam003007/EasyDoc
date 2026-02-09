using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Exceptions;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Errors;
using EasyDoc.Application.Services;
using EasyDoc.SharedKernel;

namespace EasyDoc.Application.CQRS.Doctors.Commands;

public record DeleteMeCommand() : ICommand;

internal class DeleteMeCommandHandler : ICommandHandler<DeleteMeCommand>
{
    private readonly IUserContext _userContext;
    private readonly DoctorsService _doctorsService;
    private readonly ISignInService _signInService;

    public DeleteMeCommandHandler(IUserContext userContext, DoctorsService doctorsService, ISignInService signInService)
    {
        _userContext = userContext;
        _doctorsService = doctorsService;
        _signInService = signInService;
    }
    public async Task<Result> HandleAsync(DeleteMeCommand command, CancellationToken cancellationToken = default)
    {
        var doctorId = _userContext.DoctorId;

        var result = await _doctorsService.DeleteDoctorSoftAsync(doctorId, cancellationToken);

        if (!result.IsSuccess && result.Error.Code == DoctorErrors.NotFoundCode)
            throw new CurrentUserNotFoundException(_userContext.UserId);

        await _signInService.SignOutAsync();

        return result;
    }
}