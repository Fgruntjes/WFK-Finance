using App.Lib.Data.Entity;

namespace App.TransactionCategory.Interface;

public interface ITransactionCategoryService
{
    public Task<TransactionCategoryEntity> CreateAsync(
        string name,
        TransactionCategoryGroup group,
        Guid? parentId,
        int sortOrder, string? description,
        CancellationToken cancellationToken = default);

    public Task<TransactionCategoryEntity> UpdateAsync(
        Guid id,
        string name,
        TransactionCategoryGroup group,
        Guid? parentId,
        int sortOrder,
        string? description,
        CancellationToken cancellationToken = default);
}
