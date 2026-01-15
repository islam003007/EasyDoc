using EasyDoc.Application.Abstractions.Messaging;
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
