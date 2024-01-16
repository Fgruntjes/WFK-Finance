using App.Institution.Exception;
using App.Institution.Interface;
using App.Lib.Data;
using App.Lib.Data.Entity;
using App.Lib.ServiceBus;
using App.Lib.ServiceBus.Messages.Institution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace App.Institution.Service;

internal class TransactionImportQueueService : ITransactionImportQueueService
{
    private readonly ILogger<TransactionImportQueueService> _log;
    private readonly DatabaseContext _database;
    private readonly IServiceBus _serviceBus;
    private readonly IClock _clock;

    public TransactionImportQueueService(
        ILogger<TransactionImportQueueService> logger,
        DatabaseContext database,
        IServiceBus serviceBus,
        IClock clock)
    {
        _log = logger;
        _database = database;
        _serviceBus = serviceBus;
        _clock = clock;
    }

    public async Task QueueAllAccountsAsync(CancellationToken cancellationToken = default)
    {
        var pageNumber = 0;
        var pageSize = 1000;

        while (true)
        {
            var institutionAccounts = await _database.InstitutionAccounts
                .Where(x => x.ImportStatus != ImportStatus.Queued)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            if (!institutionAccounts.Any())
            {
                break;
            }

            foreach (var institutionAccount in institutionAccounts)
            {
                await QueueAccountAsync(institutionAccount, cancellationToken);
            }
            await _database.SaveChangesAsync(cancellationToken);

            pageNumber++;
        }
    }

    public async Task<bool> QueueAccountAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var account = await _database.InstitutionAccounts.SingleOrDefaultAsync(x => x.Id == accountId, cancellationToken)
            ?? throw new InstitutionAccountNotFoundException(accountId);

        var result = await QueueAccountAsync(account, cancellationToken);
        if (result)
        {
            await _database.SaveChangesAsync(cancellationToken);
        }
        return result;
    }

    private async Task<bool> QueueAccountAsync(InstitutionAccountEntity account, CancellationToken cancellationToken = default)
    {
        var now = _clock.GetCurrentInstant();
        var importDelay = Duration.FromHours(12);

        if (account.LastImportRequested != null && account.LastImportRequested + importDelay >= now)
        {
            _log.LogInformation(
                "Skipping refresh for {InstitutionAccountId} as it was refreshed at {LastImport}",
                account.Id,
                account.LastImport);
            return false;
        }

        await _serviceBus.Send(new TransactionImportJob
        {
            InstitutionConnectionAccountId = account.Id,
        }, cancellationToken);

        account.LastImportRequested = now;
        account.ImportStatus = ImportStatus.Queued;
        return true;
    }
}