using EasyDoc.SharedKernel;

namespace EasyDoc.Application.Abstractions.Authentication;

public interface ISignInService
{
    Task<Result> SignInAsync(string email, string password);
    Task SignOutAsync();
}