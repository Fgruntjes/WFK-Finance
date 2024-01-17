using App.Lib.Data;

namespace App.Backend.Dto;

public class OrganisationUserDto
{
    public required UserDto User { get; set; }
    public required OrganisationDto Organisation { get; set; }
    public required UserRole Role { get; set; }
}