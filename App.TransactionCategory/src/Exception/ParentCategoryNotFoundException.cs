namespace App.TransactionCategory.Exception;

public sealed class CategoryNotFoundException : System.Exception
{
    public CategoryNotFoundException(Guid id)
        : base("Id {Id} not found")
    {
        Data.Add("Id", id);
    }
}
