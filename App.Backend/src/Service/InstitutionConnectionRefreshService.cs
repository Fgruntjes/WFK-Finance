using System.Linq.Expressions;
using System.Security.Principal;
using App.Data;
using App.Data.Entity;
using Microsoft.EntityFrameworkCore;
using VMelnalksnis.NordigenDotNet;

namespace App.Backend.Service;

public class InstitutionConnectionRefreshService
{
    private DatabaseContext _database;
    private readonly OrganisationIdProvider _organisationIdProvider;
    private INordigenClient _nordigenClient;

    public InstitutionConnectionRefreshService(
        DatabaseContext database,
        OrganisationIdProvider organisationIdProvider,
        INordigenClient nordigenClient)
    {
        _database = database;
        _organisationIdProvider = organisationIdProvider;
        _nordigenClient = nordigenClient;
    }

    public async Task<InstitutionConnectionEntity> Refresh(
        IPrincipal user,
        string externalId,
        CancellationToken cancellationToken = default)
    {
        return await Refresh(user, e => e.ExternalId == externalId, cancellationToken);
    }

    public async Task<InstitutionConnectionEntity> Refresh(
        IPrincipal user,
        Guid connectionId,
        CancellationToken cancellationToken = default)
    {
        return await Refresh(user, e => e.Id == connectionId, cancellationToken);
    }

    private async Task<InstitutionConnectionEntity> Refresh(
        IPrincipal user,
        Expression<Func<InstitutionConnectionEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var organisationId = _organisationIdProvider.GetOrganisationId(user);
        var entity = await _database.InstitutionConnections
            .OrderBy(e => e.CreatedAt)
            .Take(1)
            .Where(predicate)
            .Where(e => e.OrganisationId == organisationId)
            .FirstAsync(cancellationToken);

        var connectionAccounts = await _nordigenClient.Requisitions.Get(Guid.Parse(entity.ExternalId), cancellationToken);
        var connectionAccountEntities = new List<InstitutionConnectionAccountEntity>();
        await Task.WhenAll(connectionAccounts.Accounts.Select(async account =>
        {
            var accountInfo = await _nordigenClient.Accounts.Get(account);
            connectionAccountEntities.Add(new InstitutionConnectionAccountEntity
            {
                Iban = accountInfo.Iban,
                ExternalId = accountInfo.Id.ToString(),
                InstitutionConnectionId = entity.Id,
            });
        }));

        await _database.InstitutionConnectionAccounts.UpsertRange(connectionAccountEntities)
            .On(i => new
            {
                i.ExternalId,
                i.InstitutionConnectionId
            })
            .NoUpdate()
            .RunAsync(cancellationToken);

        return entity;
    }
}
