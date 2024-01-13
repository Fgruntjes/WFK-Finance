using App.Lib.Data;
using App.Lib.Data.Entity;

namespace App.Backend;

public class HttpContextOrganisationIdProvider : IOrganisationIdProvider
{
    private readonly DatabaseContext _database;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDictionary<string, Guid> _storedOrganisations;
    private static readonly object _lock = new();

    public HttpContextOrganisationIdProvider(DatabaseContext database, IHttpContextAccessor httpContextAccessor)
    {
        _database = database;
        _httpContextAccessor = httpContextAccessor;
        _storedOrganisations = new Dictionary<string, Guid>();
    }

    public Guid GetOrganisationId()
    {
        var identity = _httpContextAccessor.HttpContext?.User.Identity?.Name
            ?? throw new System.Exception("No identity found");

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

                if (organisationEntity != null)
                {
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
            catch (System.Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}