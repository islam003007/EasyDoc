using EasyDoc.SharedKernel;

namespace EasyDoc.Application.Errors;

public static class AuthErrors
{
    public const string LockedOutCode = "Auth.LockedOut";
    public const string EmailNotConfirmedCode = "Auth.EmailNotConfirmed";
    public const string LoginFailedCode = "Auth.loginFailed";
    public const string InvalidTokenCode = "Auth.Token";
    public const string EmailAlreadyConfirmedCode = "Auth.EmailAlreadyConfirmed";

    public static Error LockedOut = Error.Problem(LockedOutCode,
        "You were locked out for trying to log in too many times, try again after a few minutes");
    public static Error EmailNotConfirmed => Error.Problem(EmailNotConfirmedCode, "Please Confirm your email before loging in");
    public static Error LoginFailed = Error.Problem(LoginFailedCode, "Login Failed");
    public static Error InvalidToken = Error.Problem(InvalidTokenCode, "The token you provided was invalid");
    public static Error EmailAlreadyConfirmed => Error.Problem(EmailAlreadyConfirmedCode, "Your email was already confirmed");
}
