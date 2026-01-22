using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Auth.Commands;

public record LoginCommand(string Email, string Password) : ICommand;

internal class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(X => X.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(X => X.Password)
            .NotEmpty();
    }
}

internal class LoginCommandHandler : ICommandHandler<LoginCommand>
{
    private readonly ISignInService _signInService;

    public LoginCommandHandler(ISignInService signInService)
    {
        _signInService = signInService;
    }

    public Task<Result> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        return _signInService.SignInAsync(command.Email, command.Password);
    }
}