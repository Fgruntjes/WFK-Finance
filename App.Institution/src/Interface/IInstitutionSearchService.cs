using App.Lib.Data.Entity;

namespace App.Institution.Interface;

public interface IInstitutionSearchService
{
    public Task<IEnumerable<InstitutionEntity>> Search(string countryIso2, CancellationToken cancellationToken = default);
}