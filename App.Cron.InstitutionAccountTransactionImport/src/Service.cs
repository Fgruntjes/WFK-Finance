using App.Lib.Data;
using App.Lib.Data.Entity;
using App.Lib.ServiceBus.Messages.InstitutionConnection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rebus.Bus;

namespace App.Cron.InstitutionAccountTransactionImport;

public class Service
{
    private readonly ILogger<Service> _logger;
    private readonly DatabaseContext _database;
    private readonly IBus _bus;

    public Service(ILogger<Service> logger, DatabaseContext database, IBus bus)
    {
        _logger = logger;
        _database = database;
        _bus = bus;
    }

    public async Task QueueImports(CancellationToken cancellationToken = default)
    {
        int pageNumber = 0;
        int pageSize = 1000;

        while (true)
        {
            var institutionAccounts = await _database.InstitutionAccounts
                .AsNoTracking()
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
                await _bus.Publish(new TransactionImportJob
                {
                    InstitutionConnectionAccountId = institutionAccount.Id
                });
            }

            pageNumber++;
        }
    }
}