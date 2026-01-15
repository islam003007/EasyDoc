using EasyDoc.SharedKernel;

namespace EasyDoc.Application.Errors;

public static class UserErrors
{
    public const string NotFoundCode = "Users.NotFound";
    public static Error NotFound(Guid userId) =>
        Error.NotFound(NotFoundCode, $"The User with the ID = {userId} was not found");
}
