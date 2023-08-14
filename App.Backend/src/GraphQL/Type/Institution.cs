namespace App.Backend.GraphQL.Type;

public class Institution
{
	public Guid Id { get; set; }
	public string ExternalId { get; set; } = null!;
	public string Name { get; set; } = null!;
	public string? Logo { get; set; }
	public string[] Countries { get; set; } = null!;
	public InstitutionConnection[] Connections { get; set; } = null!;
}