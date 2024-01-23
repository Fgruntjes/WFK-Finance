using App.Lib.Data.Entity;

namespace App.Backend.Dto;

public class TransactionCategoryDto
{
    public required string Name { get; set; }

    public Guid? ParentId { get; set; }

    public CategoryGroup Group { get; set; }
}