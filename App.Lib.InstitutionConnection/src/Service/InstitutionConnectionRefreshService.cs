using System.Linq.Expressions;
using App.Lib.Data;
using App.Lib.Data.Entity;
using App.Lib.InstitutionConnection.Exception;
using Microsoft.EntityFrameworkCore;
using VMelnalksnis.NordigenDotNet;

namespace App.Lib.InstitutionConnection.Service;

internal class InstitutionConnectionRefreshService : IInstitutionConnectionRefreshService
{
    private readonly DatabaseContext _database;
    private readonly IOrganisationIdProvider _organisationIdProvider;
    private readonly INordigenClient _nordigenClient;

    public InstitutionConnectionRefreshService(
        DatabaseContext database,
        IOrganisationIdProvider organisationIdProvider,
        INordigenClient nordigenClient)
    {
        _database = database;
        _organisationIdProvider = organisationIdProvider;
        _nordigenClient = nordigenClient;
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

        var connectionAccounts = await _nordigenClient.Requisitions.Get(
            Guid.Parse(entity.ExternalId),
            cancellationToken);

        if (await UpdateAccounts(entity, connectionAccounts.Accounts.ToArray()))
        {
            await _database.SaveChangesAsync(cancellationToken);
        }

        return entity;
    }

    private async Task<bool> UpdateAccounts(InstitutionConnectionEntity entity, IEnumerable<Guid> externalIdList)
    {
        var entityAccounts = entity.Accounts.ToList();
        var localIds = new HashSet<Guid>(entityAccounts.Select(a => new Guid(a.ExternalId)));
        var externalIds = new HashSet<Guid>(externalIdList);

        if (localIds.SetEquals(externalIds))
        {
            return false;
        }

        // Remove accounts that are not in the new list
        var removeAccounts = entityAccounts
            .Where(account => !externalIds.Contains(new Guid(account.ExternalId)));
        foreach (var accountEntity in removeAccounts)
        {
            entity.Accounts.Remove(accountEntity);
            _database.InstitutionConnectionAccounts.Remove(accountEntity);
        }

        // Add new accounts that are not in the current list
        var newIds = externalIds.Where(id => !localIds.Contains(id));
        foreach (var newId in newIds)
        {
            var accountInfo = await _nordigenClient.Accounts.Get(newId);
            entity.Accounts.Add(new InstitutionConnectionAccountEntity
            {
                Iban = accountInfo.Iban,
                ExternalId = accountInfo.Id.ToString(),
                InstitutionConnectionId = entity.Id,
            });
        }

        return true;
    }
}
