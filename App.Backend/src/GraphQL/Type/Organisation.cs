using GraphQL.AspNet.Attributes;

namespace App.Backend.GraphQL.Type;

public class Organisation
{
	public Guid Id { get; set; }

	[GraphField(TypeExpression = $"{nameof(String)}!")]
	public string Slug { get; set; } = null!;
}
