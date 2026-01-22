using EasyDoc.SharedKernel;

namespace EasyDoc.Application.Errors;

public static class UserErrors
{
    public const string NotFoundCode = "Users.NotFound";
    public const string NotFoundByEmailCode = "Users.NotFound.ByEmail";
    public static Error NotFound(Guid userId) =>
        Error.NotFound(NotFoundCode, $"The User with the ID = {userId} was not found");
    public static Error NotFoundByEmail(string email) =>
        Error.NotFound(NotFoundByEmailCode, $"The User with the Email = {email} was not found");
}
