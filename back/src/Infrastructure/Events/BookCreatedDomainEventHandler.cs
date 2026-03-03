using Domain.Events;
using Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Infrastructure.Services;

namespace Infrastructure.Events;

public class BookCreatedDomainEventHandler : INotificationHandler<DomainEventNotification<BookCreatedDomainEvent>>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<BookCreatedDomainEventHandler> _logger;
    private readonly AzureEmailOptions _options;

    public BookCreatedDomainEventHandler(IEmailService emailService, ILogger<BookCreatedDomainEventHandler> logger, IOptions<AzureEmailOptions> options)
    {
        _emailService = emailService;
        _logger = logger;
        _options = options.Value;
    }

    public Task Handle(DomainEventNotification<BookCreatedDomainEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        var recipient = string.IsNullOrWhiteSpace(_options.RecipientAddress)
            ? "developers@inspand.com.br"
            : _options.RecipientAddress;

        var message = $"Livro criado com sucesso. Id: {domainEvent.BookId}, Título: {domainEvent.Title}, Autor: {domainEvent.Author}";

        _emailService.SendEmail(recipient, message);
        _logger.LogInformation("Fake e-mail de criação de livro disparado para {Recipient}", recipient);

        return Task.CompletedTask;
    }
}
