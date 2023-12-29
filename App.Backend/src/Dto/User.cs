namespace App.Backend.Dto;

public class User
{
    public Guid Id { get; set; }

    public string ExternalId { get; set; } = null!;

    public IReadOnlyList<Organisation> Organisations { get; set; }

    public User()
    {
        Organisations = new List<Organisation>();
    }
}