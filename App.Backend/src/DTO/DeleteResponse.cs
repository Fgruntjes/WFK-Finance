namespace App.Backend.DTO;

public class DeleteResponse
{
    public long DeletedCount { get; init; }

    public DeleteResponse(long deletedCount)
    {
        DeletedCount = deletedCount;
    }
}