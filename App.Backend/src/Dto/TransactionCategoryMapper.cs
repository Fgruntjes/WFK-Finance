using App.Lib.Data.Entity;

namespace App.Backend.Dto;

public static class TransactionCategoryMapper
{
    public static TransactionCategoryDto ToDto(this TransactionCategoryEntity entity)
    {
        return new()
        {
            Name = entity.Name,
            ParentId = entity.ParentId,
            Group = entity.Group,
        };
    }
}