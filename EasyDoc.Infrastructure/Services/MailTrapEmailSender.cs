using EasyDoc.Domain.Entities;
using EasyDoc.Infrastructure.Options;
using Mailtrap;
using Mailtrap.Core.Validation;
using Mailtrap.Emails.Requests;
using Mailtrap.Emails.Responses;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyDoc.Infrastructure.Services;

internal class MailTrapEmailSender : IEmailSender
{
    private readonly IMailtrapClient _mailtrapClient; // TODO: this is not used. there has to be a problem here.
    private readonly MailTrapOptions _mailOptions;
    private readonly ILogger<MailTrapEmailSender> _logger;

    public MailTrapEmailSender(IMailtrapClient mailtrapClient, IOptions<MailTrapOptions> emailOptions, ILogger<MailTrapEmailSender> logger)
    {
        _mailtrapClient = mailtrapClient;
        _mailOptions = emailOptions.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var request = SendEmailRequest.Create()
            .From(_mailOptions.From)
            .Subject(subject)
            .To(email)
            .Html(htmlMessage);

        ValidationResult validationResult = request.Validate();

        if (!validationResult.IsValid)
        {
            _logger.LogError("Malformed email request:\n{ValidationResult}", validationResult.ToString("\n"));
            return;
        }

        try
        {
            SendEmailResponse? response = await _mailtrapClient
                .Test(_mailOptions.Id)
                .Send(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending email");
        }
    }
}
