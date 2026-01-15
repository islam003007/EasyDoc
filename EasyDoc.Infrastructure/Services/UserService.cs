using EasyDoc.Application.Abstractions.Data;
using EasyDoc.Application.Abstractions.Exceptions;
using EasyDoc.Application.Errors;
using EasyDoc.Infrastructure.Data.Identity;
using EasyDoc.SharedKernel;
using Microsoft.AspNetCore.Identity;
using System.Collections.Immutable;

namespace EasyDoc.Infrastructure.Services;

internal class UserService : IUserService // all of the methods in this service are supposed to run in a db transaction.
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    public UserService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
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

        return user.Id;
    }

    public async Task<Result> DeleteUserSoftAsync(Guid userId)
    {
        if (_unitOfWork.CurrentTransaction == null)
            throw new TransactionRequiredException();

        ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            return Result.Failure(UserErrors.NotFound(userId));

        var lockoutResult = await LockUserOut(user);

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

    public async Task<Result> LockUserOut(ApplicationUser user)
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
