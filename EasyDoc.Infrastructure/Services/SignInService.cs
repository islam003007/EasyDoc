using EasyDoc.Application.Abstractions.Authentication;
using EasyDoc.Application.Errors;
using EasyDoc.Infrastructure.Data.Identity;
using EasyDoc.SharedKernel;
using Microsoft.AspNetCore.Identity;

namespace EasyDoc.Infrastructure.Services;

internal class SignInService : ISignInService
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public SignInService(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<Result> SignInAsync(string email, string password)
    {
        var loginResult = await _signInManager.PasswordSignInAsync(email,
            password,
            isPersistent: true,
            lockoutOnFailure: true);

        if (loginResult.Succeeded)
        {
            return Result.Success();
        }

        else if (loginResult.IsLockedOut)
        {
            return Result.Failure(AuthErrors.LockedOut);
        }

        else if (loginResult.IsNotAllowed)
        {
            return Result.Failure(AuthErrors.EmailNotConfirmed); // could also mean phone number not confirmed or something else
        }                                                        // so this should change depending on the specific auth requirements.

        //else if (loginResult.RequiresTwoFactor)

        else
        {
            return Result.Failure(AuthErrors.LoginFailed);
        }
    }
    public Task SignOutAsync() => _signInManager.SignOutAsync();
}