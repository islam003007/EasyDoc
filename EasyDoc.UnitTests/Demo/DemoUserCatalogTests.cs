using EasyDoc.Application.Demo;
using Xunit;

namespace EasyDoc.UnitTests.Demo;

public class DemoUserCatalogTests
{
    [Theory]
    [InlineData("sara", "sara@example.com", DemoRoles.Patient)]
    [InlineData("ahmed", "ahmed@example.com", DemoRoles.Doctor)]
    [InlineData("admin", "admin@example.com", DemoRoles.Admin)]
    public void Find_ReturnsTheSeededDemoAccounts(string key, string email, string role)
    {
        var user = DemoUserCatalog.Find(key);

        Assert.NotNull(user);
        Assert.Equal(email, user.Email);
        Assert.Equal(role, user.Role);
    }

    [Fact]
    public void Find_IsCaseInsensitiveAndTrimsInput()
    {
        var user = DemoUserCatalog.Find("  SARA ");

        Assert.NotNull(user);
        Assert.Equal("sara@example.com", user.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("unknown")]
    [InlineData(null)]
    public void Find_ReturnsNullForUnknownKeys(string? key)
    {
        Assert.Null(DemoUserCatalog.Find(key));
    }

    [Fact]
    public void Catalog_HasUniqueKeysAndMapsEveryRoleToAnIdentityRole()
    {
        Assert.Equal(DemoUserCatalog.All.Count, DemoUserCatalog.All.Select(user => user.Key).Distinct().Count());

        foreach (var user in DemoUserCatalog.All)
        {
            Assert.NotNull(DemoUserCatalog.GetIdentityRole(user.Role));
        }
    }
}
