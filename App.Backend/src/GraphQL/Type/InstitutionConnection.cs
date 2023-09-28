using GraphQL.AspNet.Attributes;

namespace App.Backend.GraphQL.Type;

public class InstitutionConnection
{
	public Guid Id { get; set; }

	[GraphField(TypeExpression = $"{nameof(String)}!")]
	public string ExternalId { get; set; } = null!;

	public Guid InstitutionId { get; set; }

	[GraphField(TypeExpression = $"{nameof(Uri)}!")]
	public Uri ConnectUrl { get; set; } = null!;
}
