namespace App.Backend.Dto;

public class InstitutionConnectionCreate
{
    public required Guid InstitutionId { get; set; }

    public required Uri ReturnUrl { get; set; }
}