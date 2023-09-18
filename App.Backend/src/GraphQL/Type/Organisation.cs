using App.Backend.Data.Entity;
using GraphQL.AspNet.Attributes;

namespace App.Backend.GraphQL.Type;

public class Organisation
{
	public Guid Id { get; set; }

	[GraphField(TypeExpression = $"{nameof(String)}!")]
	public string Slug { get; set; } = null!;

	public static Organisation FromEntity(OrganisationEntity entity)
	{
		return new Organisation()
		{
			Id = entity.Id,
			Slug = entity.Slug,
		};
	}
}