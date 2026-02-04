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

    public DeleteMeCommandHandler(PatientService patientService, IUserContext userContext)
    {
        _patientService = patientService;
        _userContext = userContext;
    }

    public Task<Result> HandleAsync(DeleteMeCommand command, CancellationToken cancellationToken = default)
    {
        return _patientService.DeletePatientSoftAsync(_userContext.PatientId, cancellationToken);
    }
}