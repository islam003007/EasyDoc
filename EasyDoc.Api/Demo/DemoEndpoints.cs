using EasyDoc.Application.Demo;
using EasyDoc.Application.Options;
using EasyDoc.Infrastructure.Data;
using EasyDoc.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EasyDoc.Api.Demo;

/// <summary>
/// Demo-only endpoints. The caller maps this group only when Demo:Enabled is true.
/// </summary>
public static class DemoEndpoints
{
    public static IEndpointRouteBuilder MapDemoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/auth/demo-users", (IOptions<DemoOptions> options) =>
        {
            if (!options.Value.Enabled)
                return Results.NotFound();

            return Results.Ok(DemoUserCatalog.All);
        })
        .WithTags("demo")
        .AllowAnonymous();

        app.MapPost("/auth/demo-login", async (
            [FromBody] DemoLoginRequest request,
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
        })
        .WithTags("demo")
        .AllowAnonymous();

        // The login page calls this once. It is deliberately not a health check: a monitor
        // must not keep the free Azure SQL database awake.
        app.MapGet("/demo/warmup", async (
            ApplicationDbContext dbContext,
            IOptions<DemoOptions> options,
            CancellationToken cancellationToken) =>
        {
            if (!options.Value.Enabled)
                return Results.NotFound();

            try
            {
                await dbContext.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
                return Results.Ok(new { status = "ready" });
            }
            catch (Exception)
            {
                return Results.Json(
                    new { status = "warming" },
                    statusCode: StatusCodes.Status503ServiceUnavailable);
            }
        })
        .WithTags("demo")
        .AllowAnonymous();

        return app;
    }

    private sealed record DemoLoginRequest(string Key);
}
