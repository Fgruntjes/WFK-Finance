using System.Collections.Immutable;
using App.Backend.Data.Entity;
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

	public static Institution FromEntity(InstitutionEntity entity)
	{
		return new Institution
		{
			Id = entity.Id,
			ExternalId = entity.ExternalId,
			Name = entity.Name,
			Logo = entity.Logo,
			Country = entity.CountryIso2,
		};
	}
}