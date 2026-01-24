using EasyDoc.SharedKernel;

namespace EasyDoc.Application.Abstractions.Data;

public interface IUserService
{
    public Task<Result> DeleteUserSoftAsync(Guid UserId);
    public Task<Result> DeleteUserPermanentAsync(Guid UserId);
    public Task<Result<Guid>> CreateUserAsync(string email, string password, string role);
    public Task<Result> ConfirmUserEmailAsync(Guid userId, string token);
    public Task<Result> ResendEmailConfirmationTokenAsync(string email);
    public Task<Result> RequestPasswordResetAsync(string email);
    public Task<Result> ResetPasswordAsync(string email, string token, string newPassword);
}
