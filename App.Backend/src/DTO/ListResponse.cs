namespace App.Backend.DTO;

public class ListResponse<T>
{
    public T[] Items { get; init; } = null!;
    public long ItemsTotal { get; init; } = 0;

    public ListResponse(T[] items, long itemsTotal)
    {
        Items = items;
        ItemsTotal = itemsTotal;
    }
}