using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;

namespace EasyDoc.Infrastructure.Services;

/// <summary>
/// Development-only email sender. It logs the message instead of sending it.
/// Replace with a real provider (Resend, Mailpit SMTP, etc.) before production.
/// </summary>
internal class LogEmailSender : IEmailSender
{
    private readonly ILogger<LogEmailSender> _logger;

    public LogEmailSender(ILogger<LogEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        _logger.LogInformation(
            "Email to {Email} | Subject: {Subject}\n{Body}",
            email,
            subject,
            htmlMessage);

        return Task.CompletedTask;
    }
}
