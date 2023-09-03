using App.Backend.Data.Entity;

namespace App.Backend.GraphQL.Type;

public class InstitutionConnection
{
	public Guid Id { get; set; }
	public string ExternalId { get; set; } = null!;
	public Guid OrganisationId { get; set; }
	public Guid InstitutionId { get; set; }
	public string ConnectUrl { get; set; } = null!;
	
	public static InstitutionConnection FromEntity(InstitutionConnectionEntity entity)
	{
		return new InstitutionConnection
		{
			Id = entity.Id,
			ExternalId = entity.ExternalId,
			OrganisationId = entity.OrganisationId,
			InstitutionId = entity.InstitutionId,
			ConnectUrl = entity.ConnectUrl,
		};
	}
}