using GraphQL.AspNet.Attributes;

namespace App.Backend.GraphQL.Type;

public class User
{
    public Guid Id { get; set; }

    [GraphField(TypeExpression = $"{nameof(String)}!")]
    public string ExternalId { get; set; } = null!;

    [GraphField(TypeExpression = $"[{nameof(Organisation)}!]!")]
    public IReadOnlyList<Organisation> Organisations { get; set; }

    public User()
    {
        Organisations = new List<Organisation>();
    }
}