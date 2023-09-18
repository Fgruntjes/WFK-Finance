using System.Linq.Expressions;
using App.Backend.Data;
using App.Backend.Data.Entity;
using Microsoft.EntityFrameworkCore;
using VMelnalksnis.NordigenDotNet;

namespace App.Backend.Service;

public class InstitutionConnectionRefreshService
{
    private DatabaseContext _database;
    private IHttpContextAccessor _httpContextAccessor;
    private INordigenClient _nordigenClient;

    public InstitutionConnectionRefreshService(
        DatabaseContext database,
        IHttpContextAccessor httpContextAccessor,
        INordigenClient nordigenClient)
    {
        _database = database;
        _httpContextAccessor = httpContextAccessor;
        _nordigenClient = nordigenClient;
    }

    public async Task<InstitutionConnectionEntity> Refresh(string externalId, CancellationToken cancellationToken = default)
    {
        return await Refresh(e => e.ExternalId == externalId, cancellationToken);
    }

    public async Task<InstitutionConnectionEntity> Refresh(Guid connectionId, CancellationToken cancellationToken = default)
    {
        return await Refresh(e => e.Id == connectionId, cancellationToken);
    }

    private async Task<InstitutionConnectionEntity> Refresh(Expression<Func<InstitutionConnectionEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var organisationId = _httpContextAccessor.GetOrganisationId();
        var entity = await _database.InstitutionConnections
            .OrderBy(e => e.CreatedAt)
            .Take(1)
            .Where(predicate)
            .Where(e => e.OrganisationId == organisationId)
            .FirstAsync(cancellationToken);

        var connectionAccounts = await _nordigenClient.Requisitions.Get(Guid.Parse(entity.ExternalId));
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
                i.ExternalId
            })
            .NoUpdate()
            .RunAsync(cancellationToken);
        return entity;
    }
}
