namespace App.Lib.ServiceBus.Messages.InstitutionConnection;

public class TransactionImportJob : IMessage
{
    public Guid InstitutionConnectionAccountId { get; set; }
}