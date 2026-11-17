using EasyDoc.Application.CQRS.Auth.Queries;
using Xunit;

namespace EasyDoc.UnitTests.Auth;

public class AuthRoleResolverTests
{
    [Fact]
    public void Resolve_PrefersAdminThenDoctorThenPatient()
    {
        Assert.Equal("admin", AuthRoleResolver.Resolve(new[] { "Patient", "Doctor", "Admin" }));
        Assert.Equal("doctor", AuthRoleResolver.Resolve(new[] { "Patient", "Doctor" }));
        Assert.Equal("patient", AuthRoleResolver.Resolve(new[] { "Patient" }));
    }

    [Fact]
    public void Resolve_ReturnsEmptyForAnAccountWithoutRoles()
    {
        Assert.Equal(string.Empty, AuthRoleResolver.Resolve(Array.Empty<string>()));
    }
}
