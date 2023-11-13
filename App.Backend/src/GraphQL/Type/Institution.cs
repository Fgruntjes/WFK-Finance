using GraphQL.AspNet.Attributes;

namespace App.Backend.GraphQL.Type;

public class Institution
{
    public Guid Id { get; init; }

    [GraphField(TypeExpression = "Type!")]
    public string ExternalId { get; init; } = null!;

    [GraphField(TypeExpression = "Type!")]
    public string Name { get; init; } = null!;

    public Uri? Logo { get; init; }

    [GraphField(TypeExpression = "Type!")]
    public string Country { get; init; } = null!;
}