using App.Backend.Data.Entity;
using GraphQL.AspNet.Attributes;

namespace App.Backend.GraphQL.Type;

public class InstitutionConnectionAccount
{
	public Guid Id { get; set; }

	[GraphField(TypeExpression = $"{nameof(String)}!")]
	public string ExternalId { get; set; } = null!;

	[GraphField(TypeExpression = $"{nameof(String)}!")]
	public string Iban { get; set; } = null!;

	public static InstitutionConnectionAccount FromEntity(InstitutionConnectionAccountEntity entity)
	{
		return new InstitutionConnectionAccount
		{
			Id = entity.Id,
			ExternalId = entity.ExternalId,
			Iban = entity.Iban,
		};
	}
}