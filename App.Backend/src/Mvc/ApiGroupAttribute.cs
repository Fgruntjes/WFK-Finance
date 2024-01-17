namespace App.Backend.Mvc;

[AttributeUsage(AttributeTargets.Class)]
internal class ApiGroupAttribute : Attribute
{
    public Type? Type { get; }

    public string? Tag { get; set; }

    public ApiGroupAttribute(string tag)
    {
        Tag = tag;
    }

    public ApiGroupAttribute(Type type)
    {
        Type = type;
    }
}