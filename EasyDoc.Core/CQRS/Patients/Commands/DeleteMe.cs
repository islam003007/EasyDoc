using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Services;
using EasyDoc.SharedKernel;

namespace EasyDoc.Application.CQRS.Patients.Commands;

public record DeleteMeCommand : ICommand;

internal class DeleteMeCommandHandler : ICommandHandler<DeleteMeCommand>
{
    private readonly PatientService _patientService;
    private readonly IUserContext _userContext;
    private readonly ISignInService _signInService;

    public DeleteMeCommandHandler(PatientService patientService, IUserContext userContext, ISignInService signInService)
    {
        _patientService = patientService;
        _userContext = userContext;
        _signInService = signInService;
    }

    public async Task<Result> HandleAsync(DeleteMeCommand command, CancellationToken cancellationToken = default)
    {
        var result = await _patientService.DeletePatientSoftAsync(_userContext.PatientId, cancellationToken);

        if (result.IsSuccess)
            await _signInService.SignOutAsync();

        return result;
    }
}