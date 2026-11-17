using EasyDoc.Application.Demo;
using EasyDoc.Application.Options;
using EasyDoc.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EasyDoc.Api.Endpoints.Auth;

internal class DemoLogin : IEndpoint
{
    public Feature Feature => Feature.Auth;
    public bool IsAdminEndpoint => false;

    public class Request
    {
        public string Key { get; set; } = null!;
    }

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder app)
    {
        return app.MapPost("/demo-login", async ([FromBody] Request request,
            IOptions<DemoOptions> options,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager) =>
        {
            if (!options.Value.Enabled)
                return Results.NotFound();

            var demoUser = DemoUserCatalog.Find(request.Key);

            // Only keys from the server-side allowlist are accepted.
            if (demoUser is null)
            {
                return Results.Problem(
                    title: "Demo.InvalidUser",
                    detail: "The selected demo account is not available.",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            var user = await userManager.FindByEmailAsync(demoUser.Email);

            if (user is null || user.IsDeleted)
            {
                return Results.Problem(
                    title: "Demo.UserNotFound",
                    detail: "The selected demo account has not been seeded.",
                    statusCode: StatusCodes.Status404NotFound);
            }

            var identityRole = DemoUserCatalog.GetIdentityRole(demoUser.Role);

            if (identityRole is null || !await userManager.IsInRoleAsync(user, identityRole))
            {
                return Results.Problem(
                    title: "Demo.InvalidRole",
                    detail: "The selected demo account does not have the expected role.",
                    statusCode: StatusCodes.Status409Conflict);
            }

            await signInManager.SignInAsync(user, isPersistent: true);

            return Results.NoContent();
        });
    }
}
