using App.Lib.Data.Entity;

namespace App.Backend.Dto;

public class TransactionCategoryDto
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public Guid? ParentId { get; set; }

    public int SortOrder { get; set; }

    public TransactionCategoryGroup Group { get; set; }
}
