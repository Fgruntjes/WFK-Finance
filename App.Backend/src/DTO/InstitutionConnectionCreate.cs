namespace App.Backend.DTO;

public class InstitutionConnectionCreateRequest
{
    public string InstitutionId { get; set; } = null!;
    public Uri ReturnUri { get; set; } = null!;
}
