namespace App.Lib.Data.Exception;

public sealed class UniqueConstraintException : System.Exception
{
    public UniqueConstraintException(string message = "Unique constraint violation", System.Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
