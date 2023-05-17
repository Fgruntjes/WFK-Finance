using App.Backend.Data.Entity;

namespace App.Backend.DTO;

public class Institution
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
    public Uri? Logo { get; init; } = null!;

    internal static Institution FromEntity(InstitutionEntity institution)
    {
        return new Institution
        {
            Id = institution.Id.ToString(),
            Name = institution.Name,
            Logo = institution.Logo
        };
    }
}
