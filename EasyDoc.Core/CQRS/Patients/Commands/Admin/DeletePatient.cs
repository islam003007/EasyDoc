using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Services;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Patients.Commands.Admin;

public record DeletePatientCommand(Guid UserId) : ICommand;

internal class DeletePatientCommandValidator : AbstractValidator<DeletePatientCommand>
{
    public DeletePatientCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}

internal class DeletePatientCommandHandle : ICommandHandler<DeletePatientCommand>
{
    private readonly PatientService _patientService;

    public DeletePatientCommandHandle(PatientService patientService)
    {
        _patientService = patientService;
    }

    public Task<Result> HandleAsync(DeletePatientCommand command, CancellationToken cancellationToken = default)
    {
        return _patientService.DeletePatientSoftAsync(command.UserId, cancellationToken);
    }
}