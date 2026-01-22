using EasyDoc.Domain.Entities;
using EasyDoc.Infrastructure.Options;
using Mailtrap;
using Mailtrap.Core.Validation;
using Mailtrap.Emails.Requests;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyDoc.Infrastructure.Services;

internal class MailTrapEmailSender : IEmailSender
{
    private readonly IMailtrapClient _mailtrapClient;
    private readonly EmailOptions _emailOptions;
    private readonly ILogger<MailTrapEmailSender> _logger;

    public MailTrapEmailSender(IMailtrapClient mailtrapClient, IOptions<EmailOptions> emailOptions, ILogger<MailTrapEmailSender> logger)
    {
        _mailtrapClient = mailtrapClient;
        _emailOptions = emailOptions.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var request = SendEmailRequest.Create()
            .From(_emailOptions.From)
            .Subject(subject)
            .To(email)
            .Html(htmlMessage);

        ValidationResult validationResult = request.Validate();

        if (!validationResult.IsValid)
        {
            _logger.LogError("Malformed email request:\n{ValidationResult}", validationResult.ToString("\n"));
            return;
        }
    }
}
