using App.Institution.Service;
using App.Lib.ServiceBus;
using App.Lib.ServiceBus.Messages.InstitutionConnection;

namespace App.Institution.Job.TransactionImport;

public class MessageHandler : IMessageHandler<TransactionImportJob>
{
    private readonly ITransactionImportService _transactionImportService;

    public MessageHandler(ITransactionImportService transactionImportService)
    {
        _transactionImportService = transactionImportService;
    }

    public async Task Handle(TransactionImportJob message)
    {
        await _transactionImportService.ImportAsync(message.InstitutionConnectionAccountId);
    }
}