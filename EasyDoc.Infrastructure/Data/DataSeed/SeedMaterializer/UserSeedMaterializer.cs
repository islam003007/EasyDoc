using EasyDoc.Infrastructure.Data.Identity;

namespace EasyDoc.Infrastructure.Data.DataSeed.SeedMaterializer;

internal class UserSeedMaterializer : SeedMaterializerBase<ApplicationUser>
{
    public string Email { get; set; } = null!;
    public override ApplicationUser ToDomainObject()
    {
        var user = new ApplicationUser(Email)
        {
            EmailConfirmed = true,
        };

        SetDomainObjectId(user);

        return user;
    }
}
