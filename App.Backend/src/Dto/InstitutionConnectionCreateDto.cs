namespace App.Backend.Dto;

public class InstitutionConnectionCreateDto
{
    public required Guid InstitutionId { get; set; }

    public required Uri ReturnUrl { get; set; }
}