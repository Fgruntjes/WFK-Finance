using App.Backend.Data.Entity;

namespace App.Backend.GraphQL.Type;

public class Organisation
{
	public Guid Id { get; set; }
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