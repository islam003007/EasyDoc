using Microsoft.AspNetCore.Identity;

namespace EasyDoc.Infrastructure.Data.Identity;

public class ApplicationRole: IdentityRole<Guid>
{
    public ApplicationRole():base() { }
    public ApplicationRole(string roleName):base(roleName) { }
}
