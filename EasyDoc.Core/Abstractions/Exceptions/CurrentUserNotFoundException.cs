namespace EasyDoc.Application.Abstractions.Exceptions;

public class CurrentUserNotFoundException : AppException
{
    public CurrentUserNotFoundException(Guid userId)
      : base("Users.NotFound.Current", "Either current logged in user or user profile can not be found.", new { UserId = userId }) { }
}
