using App.Lib.Data;
using App.Lib.InstitutionConnection.Service;
using App.Lib.ServiceBus;
using App.Lib.ServiceBus.Messages;

namespace App.Job.InstitutionAccountTransactionImport;

public class MessageHandler : IMessageHandler<InstitutionAccountTransactionImportJob>
{
    private readonly ITransactionImportService _transactionImportService;

    public MessageHandler(ITransactionImportService transactionImportService)
    {
        _transactionImportService = transactionImportService;
    }

    public async Task Handle(InstitutionAccountTransactionImportJob message)
    {
        await _transactionImportService.ImportAsync(message.InstitutionConnectionAccountId);
    }
}