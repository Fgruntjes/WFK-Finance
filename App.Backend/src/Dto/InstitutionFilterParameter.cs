namespace App.Backend.Dto;

public class InstitutionFilterParameter
{
    public string? CountryIso2 { get; set; }
    public ICollection<Guid>? Id { get; set; }
}