using System.Linq.Expressions;
using App.Institution.Interface;
using App.Lib.Data;
using App.Lib.Data.Entity;
using App.Lib.Data.Exception;
using Medallion.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VMelnalksnis.NordigenDotNet;

namespace App.Institution.Service;

internal class InstitutionConnectionRefreshService : IInstitutionConnectionRefreshService
{
    private readonly DatabaseContext _database;
    private readonly IOrganisationIdProvider _organisationIdProvider;
    private readonly INordigenClient _nordigenClient;
    private readonly ILogger<InstitutionConnectionRefreshService> _log;
    private readonly IDistributedLockProvider _lockProvider;
    private readonly ITransactionImportQueueService _transactionImportQueueService;

    public InstitutionConnectionRefreshService(
        DatabaseContext database,
        IOrganisationIdProvider organisationIdProvider,
        INordigenClient nordigenClient,
        ILogger<InstitutionConnectionRefreshService> logger,
        IDistributedLockProvider distributedLockProvider,
        ITransactionImportQueueService transactionImportQueueService)
    {
        _database = database;
        _organisationIdProvider = organisationIdProvider;
        _nordigenClient = nordigenClient;
        _log = logger;
        _lockProvider = distributedLockProvider;
        _transactionImportQueueService = transactionImportQueueService;
    }

    public async Task<InstitutionConnectionEntity> Refresh(
        string externalId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await Refresh(e => e.ExternalId == externalId, cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            throw new EntityNotFoundException(externalId, exception);
        }
    }

    public async Task<InstitutionConnectionEntity> Refresh(
        Guid connectionId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await Refresh(e => e.Id == connectionId, cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            throw new EntityNotFoundException(connectionId, exception);
        }
    }

    private async Task<InstitutionConnectionEntity> Refresh(
        Expression<Func<InstitutionConnectionEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var organisationId = _organisationIdProvider.GetOrganisationId();
        var entityId = await _database.InstitutionConnections
            .Where(predicate)
            .Where(e => e.OrganisationId == organisationId)
            .Select(e => e.Id)
            .FirstAsync(cancellationToken);

        using (_lockProvider.AcquireLock($"InstitutionConnectionEntity:Refresh:{entityId}", TimeSpan.FromMinutes(2), cancellationToken))
        {
            var entity = await _database.InstitutionConnections
                .Where(e => e.Id == entityId)
                .Include(e => e.Accounts)
                .FirstAsync(cancellationToken);

            // Update accounts
            await UpdateAccounts(entity, cancellationToken);

            // Publish 
            foreach (var account in entity.Accounts)
            {
                await _transactionImportQueueService.QueueAccountAsync(account.Id, cancellationToken);
            }

            return entity;
        }
    }

    private async Task UpdateAccounts(InstitutionConnectionEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity.Accounts.Any())
        {
            _log.LogInformation(
                "Skipping update accounts for {InstitutionConnectionId} it already has accounts, to update remove the connection and recreate",
                entity.Id);
            return;
        }

        var requisition = await _nordigenClient.Requisitions.Get(Guid.Parse(entity.ExternalId), cancellationToken);
        foreach (var newId in requisition.Accounts)
        {
            var accountInfo = await _nordigenClient.Accounts.Get(newId);
            entity.Accounts.Add(new InstitutionAccountEntity
            {
                Iban = accountInfo.Iban,
                ExternalId = accountInfo.Id.ToString(),
                InstitutionConnectionId = entity.Id,
            });
        }

        await _database.SaveChangesAsync(cancellationToken);
    }
}
