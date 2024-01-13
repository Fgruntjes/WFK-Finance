namespace App.Backend.Mvc;

[AttributeUsage(AttributeTargets.Class)]
internal class ApiGroupAttribute : Attribute
{
    public Type Type { get; }

    public ApiGroupAttribute(Type type)
    {
        Type = type;
    }
}