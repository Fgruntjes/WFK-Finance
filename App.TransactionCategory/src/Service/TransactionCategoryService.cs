using App.Lib.Data;
using App.Lib.Data.Entity;
using App.Lib.Data.Exception;
using App.Lib.Data.EntityFramework;
using App.TransactionCategory.Interface;
using Microsoft.EntityFrameworkCore;

namespace App.TransactionCategory.Service;

class TransactionCategoryService : ITransactionCategoryService
{
    private readonly DatabaseContext _database;
    private readonly IOrganisationIdProvider _organisationIdProvider;

    public TransactionCategoryService(DatabaseContext database, IOrganisationIdProvider organisationIdProvider)
    {
        _database = database;
        _organisationIdProvider = organisationIdProvider;
    }

    public async Task<TransactionCategoryEntity> CreateAsync(
        string name,
        TransactionCategoryGroup group,
        Guid? parentId,
        int sortOrder,
        string? description,
        CancellationToken cancellationToken = default)
    {
        var entity = new TransactionCategoryEntity()
        {
            OrganisationId = _organisationIdProvider.GetOrganisationId(),
            Name = name,
            ParentId = parentId,
            Group = group,
            SortOrder = sortOrder,
            Description = description,
        };

        await _database.AddAsync(entity, cancellationToken);
        await SaveAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<TransactionCategoryEntity> UpdateAsync(
        Guid id,
        string name,
        TransactionCategoryGroup group,
        Guid? parentId,
        int sortOrder,
        string? description,
        CancellationToken cancellationToken = default)
    {
        var entity = await FindEntityAsync(id, cancellationToken);

        // Maybe use a mapper if we get too much of this stuff
        entity.Name = name;
        entity.Group = group;
        entity.ParentId = parentId;
        entity.SortOrder = sortOrder;
        entity.Description = description;

        await SaveAsync(entity, cancellationToken);
        return entity;
    }

    private async Task<TransactionCategoryEntity> FindEntityAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _database.TransactionCategory
            .Where(e => e.Id == id)
            .Where(e => e.OrganisationId == _organisationIdProvider.GetOrganisationId())
            .FirstOrDefaultAsync(cancellationToken) ?? throw new EntityNotFoundException(id);
    }

    private async Task SaveAsync(TransactionCategoryEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity.ParentId.HasValue)
        {
            var parentExists = await _database.TransactionCategory
                .Where(x => x.Id == entity.ParentId)
                .Where(x => x.OrganisationId == _organisationIdProvider.GetOrganisationId())
                .AnyAsync(cancellationToken);

            if (!parentExists)
            {
                throw new EntityNotFoundException(entity.ParentId.Value);
            }
        }

        try
        {
            await _database.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
        {
            if (exception.IsUniqueConstraintViolation())
            {
                throw new UniqueConstraintException(innerException: exception);
            }
            throw;
        }
    }
}
