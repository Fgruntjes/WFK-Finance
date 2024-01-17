namespace App.Lib.ServiceBus.Messages.Institution;

public class TransactionImportJob : IMessage
{
    public Guid InstitutionConnectionAccountId { get; set; }
}