namespace App.Backend.Dto;

public class Institution
{
    public Guid Id { get; init; }

    public required string ExternalId { get; init; }

    public required string Name { get; init; }

    public Uri? Logo { get; init; }

    public required string CountryIso2 { get; init; }
}