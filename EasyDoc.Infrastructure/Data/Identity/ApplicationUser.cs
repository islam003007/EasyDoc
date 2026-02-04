using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Identity;

namespace EasyDoc.Infrastructure.Data.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public bool IsDeleted { get; private set; } = false;
    private ApplicationUser() // for ef core
    {
        
    }
    public ApplicationUser(string email)
    {
        UserName = Guid.CreateVersion7().ToString("N"); // the "N" is just for format.
        SetEmail(email);
    }

    private void SetEmail(string email)
    {
        Guard.Against.NullOrWhiteSpace(email);
        Email = email;
    }

    public void SetDeletedState(bool isDeleted)
    {
        IsDeleted = isDeleted;
    }
}
