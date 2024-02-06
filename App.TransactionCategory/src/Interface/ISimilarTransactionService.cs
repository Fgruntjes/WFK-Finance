using App.Lib.Data.Entity;

namespace App.TransactionCategory.Interface;

public interface ISimilarTransactionService
{
    public Task<ICollection<InstitutionAccountTransactionEntity>> Find(Guid id, CancellationToken cancellationToken = default);
}