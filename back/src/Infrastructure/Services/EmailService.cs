using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly AzureEmailOptions _options;

    public EmailService(ILogger<EmailService> logger, IOptions<AzureEmailOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public void SendEmail(string to, string message)
    {
        var configuredRecipient = string.IsNullOrWhiteSpace(_options.RecipientAddress) ? to : _options.RecipientAddress;

        _logger.LogInformation(
            "[FAKE-AZURE-EMAIL] ConnectionStringConfigured={ConnectionStringConfigured}; Sender={Sender}; Recipient={Recipient}; Message={Message}",
            !string.IsNullOrWhiteSpace(_options.ConnectionString),
            string.IsNullOrWhiteSpace(_options.SenderAddress) ? "not-configured" : _options.SenderAddress,
            configuredRecipient,
            message);
    }
}