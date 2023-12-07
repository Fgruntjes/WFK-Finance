using App.Lib.ServiceBus.Messages;
using Rebus.Handlers;

class MessageHandler : IHandleMessages<InstitutionAccountTransactionImportJob>
{
    public Task Handle(InstitutionAccountTransactionImportJob message)
    {
        throw new NotImplementedException();
    }
}