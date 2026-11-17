using EasyDoc.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Xunit;

namespace EasyDoc.UnitTests.Auth;

public class UserContextTests
{
    [Fact]
    public void Roles_ReturnsDistinctRoleClaims()
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, "Patient"),
                new Claim(ClaimTypes.Role, "Patient"),
                new Claim(ClaimTypes.Role, "Doctor")
            }, "test"))
        };

        var userContext = new UserContext(new HttpContextAccessor { HttpContext = httpContext });

        Assert.Equal(new[] { "Doctor", "Patient" }, userContext.Roles.OrderBy(role => role));
    }

    [Fact]
    public void Roles_IsEmptyWhenTheRequestIsNotAuthenticated()
    {
        var userContext = new UserContext(new HttpContextAccessor());

        Assert.Empty(userContext.Roles);
    }
}
