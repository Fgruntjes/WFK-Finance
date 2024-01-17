namespace App.Backend.Dto;

public class UserDto
{
    public required Guid Id { get; set; }

    public required string ExternalId { get; set; }

    public required IReadOnlyList<OrganisationDto> Organisations { get; set; }

    public UserDto()
    {
        Organisations = new List<OrganisationDto>();
    }
}