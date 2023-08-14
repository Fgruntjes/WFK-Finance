namespace App.Backend.GraphQL.Type;

public class Organisation
{
	public string Id { get; set; } = null!;
	public string Slug { get; set; } = null!;
	public OrganisationUser[] Users { get; set; } = null!;
	public InstitutionConnection[] InstitutionConnections { get; set; } = null!;
}