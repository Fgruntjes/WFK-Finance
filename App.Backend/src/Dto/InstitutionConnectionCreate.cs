using System.ComponentModel.DataAnnotations;

namespace App.Backend.Dto;

public class InstitutionConnectionCreate
{
    [Required]
    public Guid InstitutionId { get; set; }

    [Required]
    public required Uri ReturnUrl { get; set; }
}