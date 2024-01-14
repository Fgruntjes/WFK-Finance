using App.Institution.Service;
using App.Lib.ServiceBus;
using App.Lib.ServiceBus.Messages.Institution;

namespace App.Institution.Job.TransactionImportEachAccount;

public class MessageHandler : IMessageHandler<TransactionImportEachAccountJob>
{
    private readonly ITransactionImportService _transactionImportService;

    public MessageHandler(ITransactionImportService transactionImportService)
    {
        _transactionImportService = transactionImportService;
    }

    public async Task Handle(TransactionImportEachAccountJob _)
    {
        await _transactionImportService.QueueAllAccountsAsync();
    }
}