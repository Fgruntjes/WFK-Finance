using App.Lib.ServiceBus;
using App.Lib.ServiceBus.Messages;
using Microsoft.Extensions.Logging;

namespace App.Job.InstitutionAccountTransactionImport;

public class MessageHandler : IMessageHandler<InstitutionAccountTransactionImportJob>
{
    private readonly ILogger<MessageHandler> _logger;

    public MessageHandler(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<MessageHandler>();
    }

    public Task Handle(InstitutionAccountTransactionImportJob message)
    {
        _logger.LogInformation("Handled message: {Message}", message);

        return Task.CompletedTask;
    }
}