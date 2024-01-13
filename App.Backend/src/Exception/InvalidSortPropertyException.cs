namespace App.Backend.Exception;

public class InvalidPropertyException : System.Exception
{
    public InvalidPropertyException(string property, Type type)
        : this(property, type, null)
    {
    }

    public InvalidPropertyException(string property, Type type, System.Exception? innerException)
        : base("Property {Property} is not found on type {Type}", innerException)
    {
        Data.Add("Property", property);
        Data.Add("Type", type.Name);
    }
}