using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Auth.Commands;

public record ResetPasswordCommand(string Email, string Token, string NewPassword) : ICommand;

internal class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Token)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty();
    }
}

internal class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand>
{
    private readonly IUserService _userService;

    public ResetPasswordCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public Task<Result> HandleAsync(ResetPasswordCommand command, CancellationToken cancellationToken = default)
    {
        return _userService.ResetPasswordAsync(command.Email, command.Token, command.NewPassword);
    }
}