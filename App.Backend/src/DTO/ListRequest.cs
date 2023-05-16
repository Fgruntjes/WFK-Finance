namespace App.Backend.DTO;

public class ListRequest
{
    private const int DEFAULT_LIMIT = 50;
    private const int DEFAULT_START = 0;

    public int Start { get; init; }
    public int Limit { get; init; }
    public string? Where { get; init; }
    public string? Sort { get; init; }

    public ListRequest(
        int? start = DEFAULT_START,
        int? limit = DEFAULT_LIMIT,
        string? where = null,
        string? sort = null)
    {
        Start = start ?? DEFAULT_START;
        Limit = limit ?? DEFAULT_LIMIT;
        Where = where;
        Sort = sort;
    }
}