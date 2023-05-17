namespace App.Backend.DTO;

public class ListRequest
{
    // TODO add validation
    public int Skip { get; init; } = 0;
    public int Limit { get; init; } = 0;
}