namespace App.Backend.Dto;

public class InstitutionConnectionInputDto
{
    public required Guid InstitutionId { get; set; }

    public required Uri ReturnUrl { get; set; }
}