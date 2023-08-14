namespace App.Backend.GraphQL.Type;

public class InstitutionConnection
{
	public string Id { get; set; } = null!;
	public string ExternalId { get; set; } = null!;
	public Organisation Organisation { get; set; } = null!;
	public int OrganisationId { get; set; }
	public Institution Institution { get; set; } = null!;
	public int InstitutionId { get; set; }
	public string ConnectUrl { get; set; } = null!;
	public InstitutionConnectionAccount[] Accounts { get; set; } = null!;
}