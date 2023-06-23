namespace App.Backend.DTO;

public class DeleteRequest
{
    // TODO add validation
    public string[] Ids { get; init; } = null!;
}