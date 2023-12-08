using App.Lib.Data.Entity;

namespace App.Lib.InstitutionConnection.Service;

public interface IInstitutionConnectionRefreshService
{
    public Task<InstitutionConnectionEntity> Refresh(
        string externalId,
        CancellationToken cancellationToken = default);

    public Task<InstitutionConnectionEntity> Refresh(
        Guid connectionId,
        CancellationToken cancellationToken = default);
}