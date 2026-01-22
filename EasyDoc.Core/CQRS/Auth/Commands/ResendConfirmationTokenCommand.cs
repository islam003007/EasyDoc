using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Auth.Commands;

public record ResendConfirmationTokenCommand(string Email) : ICommand;

internal class ResendConfirmationTokenCommandValidator : AbstractValidator<ResendConfirmationTokenCommand>
{
    public ResendConfirmationTokenCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}

internal class ResendConfirmationTokenCommandHandler : ICommandHandler<ResendConfirmationTokenCommand>
{
    private readonly IUserService _userService;

    public ResendConfirmationTokenCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public Task<Result> HandleAsync(ResendConfirmationTokenCommand command, CancellationToken cancellationToken = default)
    {
        return _userService.ResendEmailConfirmationToken(command.Email);
    }
}