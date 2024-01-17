namespace App.Institution.Interface;

public interface ITransactionImportQueueService
{
    public Task QueueAllAccountsAsync(CancellationToken cancellationToken = default);

    public Task<bool> QueueAccountAsync(Guid accountId, CancellationToken cancellationToken = default);
}
