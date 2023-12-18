namespace App.Lib.ServiceBus.Messages;

public class InstitutionAccountTransactionImportJob : IMessage
{
    public Guid InstitutionConnectionAccountId { get; set; }
}