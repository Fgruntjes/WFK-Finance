using System.Linq.Expressions;
using App.Lib.Data;
using App.Lib.Data.Entity;
using App.Lib.InstitutionConnection.Exception;
using App.Lib.ServiceBus;
using App.Lib.ServiceBus.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using VMelnalksnis.NordigenDotNet;

namespace App.Lib.InstitutionConnection.Service;

internal class InstitutionConnectionRefreshService : IInstitutionConnectionRefreshService
{
    private readonly DatabaseContext _database;
    private readonly IOrganisationIdProvider _organisationIdProvider;
    private readonly INordigenClient _nordigenClient;
    private readonly IServiceBus _serviceBus;
    private readonly IClock _clock;
    private readonly ILogger<InstitutionConnectionRefreshService> _log;

    public InstitutionConnectionRefreshService(
        DatabaseContext database,
        IOrganisationIdProvider organisationIdProvider,
        INordigenClient nordigenClient,
        IServiceBus serviceBus,
        IClock clock,
        ILoggerFactory loggerFactory)
    {
        _database = database;
        _organisationIdProvider = organisationIdProvider;
        _nordigenClient = nordigenClient;
        _serviceBus = serviceBus;
        _clock = clock;
        _log = loggerFactory.CreateLogger<InstitutionConnectionRefreshService>();
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
            throw new InstitutionConnectionNotFoundException(externalId, exception);
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
            throw new InstitutionConnectionNotFoundException(connectionId, exception);
        }
    }

    private async Task<InstitutionConnectionEntity> Refresh(
        Expression<Func<InstitutionConnectionEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var organisationId = _organisationIdProvider.GetOrganisationId();
        var entity = await _database.InstitutionConnections
            .Where(predicate)
            .Where(e => e.OrganisationId == organisationId)
            .Include(e => e.Accounts)
            .OrderBy(e => e.CreatedAt)
            .Take(1)
            .FirstAsync(cancellationToken);

        // Update accounts
        await UpdateAccounts(entity, cancellationToken);

        // Publish 
        var now = _clock.GetCurrentInstant();
        var importDelay = Duration.FromHours(12);
        foreach (var account in entity.Accounts)
        {
            if (account.LastImportRequested != null && account.LastImportRequested + importDelay > now)
            {
                _log.LogInformation(
                    "Skipping refresh for {InstitutionAccountId} as it was refreshed at {LastImport}",
                    account.Id,
                    account.LastImport);
                continue;
            }

            await _serviceBus.Send(new InstitutionAccountTransactionImportJob
            {
                InstitutionConnectionAccountId = account.Id,
            }, cancellationToken);

            account.LastImportRequested = _clock.GetCurrentInstant();
            account.ImportStatus = ImportStatus.Queued;
        }

        await _database.SaveChangesAsync(cancellationToken);
        return entity;
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
    }
}
