namespace App.Backend.GraphQL.Type;

public class InstitutionConnectionAccount
{
	public string Id { get; set; } = null!;
	public string ExternalId { get; set; } = null!;
	public InstitutionConnection InstitutionConnection { get; set; } = null!;
	public string OwnerName { get; set; } = null!;
	public string Iban { get; set; } = null!;
}