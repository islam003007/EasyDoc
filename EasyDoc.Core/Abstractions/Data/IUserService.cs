using EasyDoc.SharedKernel;

namespace EasyDoc.Application.Abstractions.Data;

public interface IUserService
{
    public Task<Result> DeleteUserSoftAsync(Guid UserId);
    public Task<Result> DeleteUserPermanentAsync(Guid UserId);
    public Task<Result<Guid>> CreateUserAsync(string email, string password, string role);
}
