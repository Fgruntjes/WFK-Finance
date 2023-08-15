using App.Backend.Data.Entity;

namespace App.Backend.GraphQL.Type;

public class Institution
{
	public Guid Id { get; init; }
	public string ExternalId { get; init; } = null!;
	public string Name { get; init; } = null!;
	public string? Logo { get; init; }
	public string[] Countries { get; init; } = null!;

	public static Institution FromEntity(InstitutionEntity entity)
	{
		return new Institution
		{
			Id = entity.Id,
			ExternalId = entity.ExternalId,
			Name = entity.Name,
			Logo = entity.Logo,
			Countries = entity.Countries
				.Select(e => e.Iso3)
				.ToArray(),
		};
	}
}