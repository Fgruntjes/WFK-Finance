using App.Backend.Data;
using App.Backend.Data.Entity;
using MongoDB.Bson;
using MongoDB.Driver;

namespace App.Backend.Service;

public class TenantService
{
    private const string DefaultTenantName = "Default";
    private readonly DatabaseContext _databaseContext;

    public TenantService(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<TenantEntity> GetOrCreate(ObjectId userId, string name = DefaultTenantName, CancellationToken cancellationToken = default)
    {
        var tenantCollection = _databaseContext.Tenants;
        var existingTenantsCursor = await tenantCollection.FindAsync(
            (tenant) => tenant.Name == name && tenant.Users.Contains(userId),
            new() { Limit = 1 },
            cancellationToken);

        var existingTenant = await existingTenantsCursor.FirstOrDefaultAsync(cancellationToken);
        if (existingTenant != null)
        {
            return existingTenant;
        }

        var entity = new TenantEntity
        {
            Name = name,
            Users = new[] { userId }
        };
        await tenantCollection.InsertOneAsync(entity, cancellationToken: cancellationToken);

        return entity;
    }

    public async Task DeleteTenant(ObjectId tenantId, CancellationToken cancellationToken)
    {
        await Task.WhenAll(
            _databaseContext.Tenants.DeleteManyAsync((e) => e.Id == tenantId, cancellationToken),
            _databaseContext.InstitutionConnections.DeleteManyAsync((e) => e.TenantId == tenantId, cancellationToken)
        );
    }
}