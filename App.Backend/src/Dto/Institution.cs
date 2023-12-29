namespace App.Backend.Dto;

public class Institution
{
    public Guid Id { get; init; }

    public string ExternalId { get; init; } = null!;

    public string Name { get; init; } = null!;

    public Uri? Logo { get; init; }

    public string Country { get; init; } = null!;
}