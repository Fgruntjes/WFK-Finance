namespace App.Backend.Dto;

public class ListResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = null!;
    public int TotalCount { get; set; }
}