using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Exceptions;
using EasyDoc.Application.Errors;
using EasyDoc.Infrastructure.Data.Identity;
using EasyDoc.SharedKernel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace EasyDoc.Infrastructure.Services;

internal class UserService : IUserService // some of the methods in this service are supposed to run in a db transaction.
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserNotificationService _userNotificationService;

    public UserService(UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork,
        UserNotificationService userNotificationService)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _userNotificationService = userNotificationService;
    }

    public async Task<Result<Guid>> CreateUserAsync(string email, string password, string role)
    {
        if (_unitOfWork.CurrentTransaction == null)
            throw new TransactionRequiredException();
        
        var user = new ApplicationUser(email);

        var createResult = await _userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
            return ToFailureResult<Guid>(createResult);

        var roleResult = await _userManager.AddToRoleAsync(user, role);

        if (!roleResult.Succeeded)
            return ToFailureResult<Guid>(roleResult);

        // Not checked so that the whole transaction doesn't fail becuase of the email failure. The user can later request another email.
        // outbox pattern would solve this.
        await _userNotificationService.SendEmailConfirmationAsync(user);  

        return user.Id;
    }

    public async Task<Result> DeleteUserSoftAsync(Guid userId)
    {
        if (_unitOfWork.CurrentTransaction == null)
            throw new TransactionRequiredException();

        ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            return Result.Failure(UserErrors.NotFound(userId));

        var lockoutResult = await LockUserOutAsync(user);

        if (!lockoutResult.IsSuccess)
            return lockoutResult;

        return Result.Success();
    }

    public async Task<Result> DeleteUserPermanentAsync(Guid userId)
    {
        if (_unitOfWork.CurrentTransaction == null)
            throw new TransactionRequiredException();

        ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            return Result.Failure(UserErrors.NotFound(userId));

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
            return ToFailureResult(result);

        return Result.Success();

    }

    private async Task<Result> LockUserOutAsync(ApplicationUser user)
    {
        if (_unitOfWork.CurrentTransaction == null)
            throw new TransactionRequiredException();

        if (!user.LockoutEnabled)
        {
            var enableResult = await _userManager.SetLockoutEnabledAsync(user, true);

            if (!enableResult.Succeeded)
                return ToFailureResult(enableResult);
        }
       
        var dateResult = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

        if (!dateResult.Succeeded)
            return ToFailureResult(dateResult);

        var stampResult = await _userManager.UpdateSecurityStampAsync(user);

        if (!stampResult.Succeeded)
            return ToFailureResult(stampResult);

        return Result.Success();
    }

    public async Task<Result> ConfirmUserEmailAsync(Guid userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return Result.Failure(UserErrors.NotFound(userId));

        try
        {
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        }
        catch (FormatException)
        {
            return Result.Failure(AuthErrors.InvalidToken);
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
            return ToFailureResult(result);

        return Result.Success();
    }

    public async Task<Result> ResendEmailConfirmationTokenAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return Result.Failure(UserErrors.NotFoundByEmail(email));

        if (user.EmailConfirmed)
            return Result.Failure(AuthErrors.EmailAlreadyConfirmed);

        await _userNotificationService.SendEmailConfirmationAsync(user);

        return Result.Success();
    }

    public async Task<Result> RequestPasswordResetAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is not null && await _userManager.IsEmailConfirmedAsync(user)) // to not reveal that that the email doesn't exist or not confirmed.
            await _userNotificationService.SendPassordResetAsync(user);

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null || await _userManager.IsEmailConfirmedAsync(user))
            return Result.Failure(AuthErrors.InvalidToken); // to not reveal that the user doesn't exist or is not confirmed.

        try
        {
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        }
        catch(FormatException)
        {
            return Result.Failure(AuthErrors.InvalidToken);
        }

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded)
            return ToFailureResult(result);

        return Result.Success();

    }

    // can be refactored to a separate class as extension methods.
    private static Result ToFailureResult(IdentityResult result) =>
        result.Succeeded ?
         throw new InvalidOperationException($"Can not convert from a successful Identity result to a failure Result")
        : Result.Failure(new MultiError(result.Errors.Select(e => Error.Problem(e.Code, e.Description))));

    private static Result<TType> ToFailureResult<TType>(IdentityResult result) =>
        result.Succeeded ?
        throw new InvalidOperationException($"Can not convert from a successful Identity result to a failure Result of type {typeof(TType).Name}")
        : Result.Failure<TType>(new MultiError(result.Errors.Select(e => Error.Problem(e.Code, e.Description))));
}
 