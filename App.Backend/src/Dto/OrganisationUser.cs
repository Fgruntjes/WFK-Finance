using App.Lib.Data;

namespace App.Backend.Dto;

public class OrganisationUser
{
    public required User User { get; set; }
    public required Organisation Organisation { get; set; }
    public required UserRole Role { get; set; }
}