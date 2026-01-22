using EasyDoc.Infrastructure.Data.Identity;
using EasyDoc.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace EasyDoc.Infrastructure.Services;

internal class UserNotificationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender<ApplicationUser> _emailSender;
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserNotificationService(
        UserManager<ApplicationUser> userManager,
        IEmailSender<ApplicationUser> emailSender,
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> SendEmailConfirmationAsync(ApplicationUser user)
    {
        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("No active HttpContext");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var routeValues = new RouteValueDictionary()
        {
            ["userId"] = user.Id,
            ["token"] = encodedToken
        };

        var confirmEmailEndpointName = "Auth.ConfirmEmail";

        var confirmEmailUrl = _linkGenerator.GetUriByName(httpContext, confirmEmailEndpointName, routeValues) ??
            throw new NotSupportedException($"Could not find endpoint named '{confirmEmailEndpointName}'.");

        // Not ensuring that the mail was actually sent was intentional as this is part of a transaction
        // and you wouldn't want the whole transaction to fail just because the email failed to sent.
        // A way better alternitive is to use the outbox pattern.
        await _emailSender.SendConfirmationLinkAsync(user, user.Email!, confirmEmailUrl);

        return Result.Success();
    }
}
