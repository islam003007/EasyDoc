using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Abstractions.Messaging;
using EasyDoc.SharedKernel;

namespace EasyDoc.Application.CQRS.Auth.Commands;

public record LogoutCommand : ICommand;


internal class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
    private readonly ISignInService _signInService;

    public LogoutCommandHandler(ISignInService signInService)
    {
        _signInService = signInService;
    }

    public async Task<Result> HandleAsync(LogoutCommand command, CancellationToken cancellationToken = default)
    {
        await _signInService.SignOutAsync();

        return Result.Success();
    }
}