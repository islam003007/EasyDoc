using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Auth.Commands;

public record ConfirmEmailCommand(Guid UserId, string Token) : ICommand;

internal class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Token)
            .NotEmpty();
    }
}

internal class ConfirmEmailCommandHandler : ICommandHandler<ConfirmEmailCommand>
{
    private readonly IUserService _userService;

    public ConfirmEmailCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public Task<Result> HandleAsync(ConfirmEmailCommand command, CancellationToken cancellationToken = default)
    {
        return _userService.ConfirmUserEmailAsync(command.UserId, command.Token);
    }
}