using System.Security.Claims;
using System.Security.Principal;
using App.Data;
using App.Data.Entity;

namespace App.Backend;

public class OrganisationIdProvider
{
    private readonly DatabaseContext _database;
    private readonly IDictionary<string, Guid> _storedOrganisations;
    private static readonly object _lock = new();

    public OrganisationIdProvider(DatabaseContext database)
    {
        _database = database;
        _storedOrganisations = new Dictionary<string, Guid>();
    }

    public Guid GetOrganisationId(IPrincipal user)
    {
        var identity = user.Identity?.Name ?? throw new Exception("No identity found");

        if (_storedOrganisations.TryGetValue(identity, out var id))
        {
            return id;
        }

        lock (_lock)
        {
            using var transaction = _database.Database.BeginTransaction();

            try
            {
                var organisationEntity = _database.Organisations
                    .Where(o => o.Slug == identity)
                    .OrderBy(o => o.Id)
                    .Take(1)
                    .FirstOrDefault();

                if (organisationEntity != null) {
                    _storedOrganisations[identity] = organisationEntity.Id;
                    return organisationEntity.Id;
                }

                var organisationId = Guid.NewGuid();
                _database.Organisations
                    .Add(new OrganisationEntity
                    {
                        Id = organisationId,
                        Slug = identity,
                    });

                _database.SaveChanges();
                transaction.Commit();

                return organisationId;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}