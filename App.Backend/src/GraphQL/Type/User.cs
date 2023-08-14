namespace App.Backend.GraphQL.Type;

public class User
{
	public string Id { get; set; } = null!;
	public string ExternalId { get; set; } = null!;
	public Organisation[] Organisations { get; set; } = null!;
}