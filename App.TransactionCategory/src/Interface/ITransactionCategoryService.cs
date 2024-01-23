using App.Lib.Data.Entity;

namespace App.TransactionCategory.Interface;

public interface ITransactionCategoryService
{
    public Task<TransactionCategoryEntity> CreateAsync(
        string name,
        CategoryGroup group,
        Guid? parentId = null,
        CancellationToken cancellationToken = default);

    public Task<TransactionCategoryEntity> UpdateAsync(
        Guid id,
        string name,
        CategoryGroup group,
        Guid? parentId,
        CancellationToken cancellationToken = default);
}
