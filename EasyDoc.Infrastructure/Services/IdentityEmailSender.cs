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
        var subject = "Password reset";

        var htmlMessage = """
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <title>Password Reset</title>
                <style>
                    body {
                        margin: 0;
                        min-height: 100vh;
                        display: flex;
                        align-items: center;
                        justify-content: center;
                        background: #f4f6f8;
                        font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Arial, sans-serif;
                    }

                    .card {
                        background: #ffffff;
                        padding: 32px 40px;
                        border-radius: 10px;
                        box-shadow: 0 10px 30px rgba(0, 0, 0, 0.08);
                        text-align: center;
                        max-width: 420px;
                        width: 100%;
                    }

                    h1 {
                        margin: 0 0 16px;
                        font-size: 22px;
                        color: #333;
                    }

                    .token {
                        display: inline-block;
                        margin-top: 12px;
                        padding: 12px 16px;
                        background: #f1f3f5;
                        border-radius: 6px;
                        font-family: monospace;
                        font-size: 16px;
                        color: #111;
                        word-break: break-all;
                    }

                    .hint {
                        margin-top: 16px;
                        font-size: 14px;
                        color: #666;
                    }
                </style>
            </head>
            <body>
                <div class="card">
                    <h1>Here is your password reset token</h1>

                    <div class="token">
                        {{token}}
                    </div>

                    <div class="hint">
                        Use this token to reset your password.  
                        It will expire shortly.
                    </div>
                </div>
            </body>
            </html>
            """.Replace("{{token}}", resetCode);

        return _emailSender.SendEmailAsync(email, subject, htmlMessage);
    }

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        throw new NotImplementedException();
    }
}