using EasyDoc.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace EasyDoc.Infrastructure.Services;

internal class IdentityEmailSender : IEmailSender<ApplicationUser>
{
    private readonly IEmailSender _emailSender;

    public IdentityEmailSender(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        var subject = "Confirm Your Email";

        var htmlMessage = $"""
        <html>
        <head>
            <meta charset="UTF-8">
            <title>Confirm Your Email</title>
        </head>
        <body style="font-family: Arial, sans-serif; line-height: 1.5; color: #333;">
            <h2>Hello,</h2>
            <p>Thank you for registering. Please confirm your email address by clicking the button below:</p>

            <p>
                <a href="{confirmationLink}" 
                   style="display: inline-block; padding: 10px 20px; background-color: #007bff; color: #ffffff; text-decoration: none; border-radius: 5px;">
                   Confirm Email
                </a>
            </p>

            <p>If the button doesn’t work, copy and paste this URL into your browser:</p>
            <p><a href="{confirmationLink}" style="color: #007bff;">{confirmationLink}</a></p>

            <p>Thank you,<br>EasyDoc</p>
        </body>
        </html>
        """;

        return _emailSender.SendEmailAsync(email, subject, htmlMessage);
    }

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        throw new NotImplementedException();
    }

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        throw new NotImplementedException();
    }
}