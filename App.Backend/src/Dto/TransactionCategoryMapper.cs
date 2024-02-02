using App.Lib.Data.Entity;

namespace App.Backend.Dto;

public static class TransactionCategoryMapper
{
    public static TransactionCategoryDto ToDto(this TransactionCategoryEntity entity)
    {
        return new()
        {
            Id = entity.Id,
            Name = entity.Name,
            ParentId = entity.ParentId,
            SortOrder = entity.SortOrder,
            Group = entity.Group,
            Description = entity.Description,
        };
    }
}