using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.Application.Services;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Doctors.Commands.Admin;

public record DeleteDoctorCommand(Guid DoctorId, bool IsSoftDelete = true) : ICommand;

internal class DeleteDoctorCommandValidator : AbstractValidator<DeleteDoctorCommand>
{
    public DeleteDoctorCommandValidator()
    {
        RuleFor(x => x.DoctorId)
            .NotEmpty();
    }
}

internal class DeleteDoctorCommandHandler : ICommandHandler<DeleteDoctorCommand>
{
    private readonly DoctorsService _doctorsService;

    public DeleteDoctorCommandHandler(DoctorsService doctorService)
    {
        _doctorsService = doctorService;
    }
    public Task<Result> HandleAsync(DeleteDoctorCommand command, CancellationToken cancellationToken = default)
    {
        if (command.IsSoftDelete)
            return _doctorsService.DeleteDoctorSoftAsync(command.DoctorId, cancellationToken);
        else
            return _doctorsService.DeleteDoctorPermanentAsync(command.DoctorId, cancellationToken);
    }
}