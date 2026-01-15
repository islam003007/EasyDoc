using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EasyDoc.Infrastructure.Data.Identity;

public static class IdentityConfig
{
    public static void ConfigureIdentity(this ModelBuilder builder)
    {
        const string schema = "identity";

        builder.Entity<ApplicationUser>(b => b.ToTable("Users", schema));
        builder.Entity<ApplicationRole>(b => b.ToTable("Roles", schema));
        builder.Entity<IdentityUserRole<Guid>>(b => b.ToTable("UserRoles", schema));
        builder.Entity<IdentityUserClaim<Guid>>(b => b.ToTable("UserClaims", schema));
        builder.Entity<IdentityUserLogin<Guid>>(b => b.ToTable("UserLogins", schema));
        builder.Entity<IdentityRoleClaim<Guid>>(b => b.ToTable("RoleClaims", schema));
        builder.Entity<IdentityUserToken<Guid>>(b => b.ToTable("UserTokens", schema));
    }
}
