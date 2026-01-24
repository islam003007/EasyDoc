using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.SharedKernel;
using FluentValidation;

namespace EasyDoc.Application.CQRS.Auth.Commands;

public record RequestPasswordResetCommand(string Email) : ICommand;

internal class RequestPasswordResetCommandValidator : AbstractValidator<RequestPasswordResetCommand>
{
    public RequestPasswordResetCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}

internal class RequestPasswordResetCommandHandler : ICommandHandler<RequestPasswordResetCommand>
{
    private readonly IUserService _userService;

    public RequestPasswordResetCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public Task<Result> HandleAsync(RequestPasswordResetCommand command, CancellationToken cancellationToken = default)
    {
        return _userService.RequestPasswordResetAsync(command.Email);
    }
}