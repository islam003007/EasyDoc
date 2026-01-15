using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Identity;

namespace EasyDoc.Infrastructure.Data.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    private ApplicationUser() // for ef core
    {
        
    }
    public ApplicationUser(string email)
    {
        UserName = Guid.CreateVersion7().ToString("N"); // the "N" is just for fromat.
        SetEmail(email);
    }

    private void SetEmail(string email)
    {
        Guard.Against.NullOrWhiteSpace(email);

        Email = email;
    }
}
