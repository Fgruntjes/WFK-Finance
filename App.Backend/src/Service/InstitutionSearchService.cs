using System.Collections.Immutable;
using App.Backend.Data;
using App.Backend.Data.Entity;
using App.Backend.Dto;
using Microsoft.EntityFrameworkCore;
using VMelnalksnis.NordigenDotNet;

namespace App.Backend.Service;

public class InstitutionSearchService
{
    private readonly DatabaseContext _database;
    private readonly INordigenClient _nordigenClient;

    public InstitutionSearchService(DatabaseContext database, INordigenClient nordigenClient)
    {
        _database = database;
        _nordigenClient = nordigenClient;
    }

    public async Task<ListResult<InstitutionEntity>> Search(string countryIso3, int offset = 0, int limit = 25, CancellationToken cancellationToken = default)
    {
        var query = _database.Institutions
            .Include(e => e.Countries)
            .Where(e => e.Countries.Any(c => c.Iso3 == countryIso3));

        var totalCount = await query.CountAsync(cancellationToken);
        if (totalCount == 0)
        {
            var list = await Refresh(countryIso3, cancellationToken);
            return new ListResult<InstitutionEntity>
            {
                Items = list.AsQueryable()
                    .Skip(offset)
                    .Take(limit)
                    .ToImmutableList(),
                TotalCount = list.Count()
            };
        }

        var result = await query
            .OrderBy(e => e.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return new ListResult<InstitutionEntity>
        {
            Items = result,
            TotalCount = totalCount
        };
    }

    public async Task<IReadOnlyList<InstitutionEntity>> Refresh(string countryIso3, CancellationToken cancellationToken)
    {
        var countryEntity = GetCountry(countryIso3);
        var institutions = (await _nordigenClient.Institutions.GetByCountry(countryIso3))
            .Select(i => new InstitutionEntity
            {
                ExternalId = i.Id,
                Name = i.Name,
                Logo = i.Logo,
            });

        await _database.Institutions
                .UpsertRange(institutions)
                .On(i => new { i.ExternalId })
                .RunAsync();

        return new List<InstitutionEntity>(institutions);
    }

    private async Task<CountryEntity> GetCountry(string countryIso3)
    {
        var country = new CountryEntity { Iso3 = countryIso3 };
        await _database.Countries
            .Upsert(country)
            .On(c => new { c.Iso3 })
            .RunAsync();

        return country;
    }
}