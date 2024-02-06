namespace App.Lib.Data.Exception;

public sealed class EntityNotFoundException : System.Exception
{
    public EntityNotFoundException(Guid id, System.Exception? innerException = null)
        : this(id.ToString(), innerException)
    { }

    public EntityNotFoundException(string id, System.Exception? innerException = null)
        : base("Id {Id} not found", innerException)
    {
        Data.Add("Id", id);
    }
}