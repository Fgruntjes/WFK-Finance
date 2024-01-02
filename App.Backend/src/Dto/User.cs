namespace App.Backend.Dto;

public class User
{
    public required Guid Id { get; set; }

    public required string ExternalId { get; set; }

    public required IReadOnlyList<Organisation> Organisations { get; set; }

    public User()
    {
        Organisations = new List<Organisation>();
    }
}