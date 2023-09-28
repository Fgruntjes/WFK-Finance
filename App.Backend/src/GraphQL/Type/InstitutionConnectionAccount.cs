using GraphQL.AspNet.Attributes;

namespace App.Backend.GraphQL.Type;

public class InstitutionConnectionAccount
{
	public Guid Id { get; set; }

	[GraphField(TypeExpression = $"{nameof(String)}!")]
	public string ExternalId { get; set; } = null!;

	[GraphField(TypeExpression = $"{nameof(String)}!")]
	public string Iban { get; set; } = null!;
}
