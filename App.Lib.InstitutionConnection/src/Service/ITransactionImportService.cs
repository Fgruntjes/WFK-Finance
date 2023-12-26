using NodaTime;

namespace App.Lib.InstitutionConnection.Service;

public interface ITransactionImportService
{
    public Task ImportAsync(Guid institutionAccountId, CancellationToken cancellationToken = default);

    public Task ImportAsync(
        Guid institutionAccountId,
        Instant lastDate,
        CancellationToken cancellationToken = default);

    public Task ImportAsync(
        Guid institutionAccountId,
        Instant firstDate,
        Instant lastDate,
        CancellationToken cancellationToken = default);
}