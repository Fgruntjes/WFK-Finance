using App.Lib.Data;

namespace App.Backend.GraphQL.Type;

public class OrganisationUser
{
    public User User { get; set; } = null!;
    public Organisation Organisation { get; set; } = null!;
    public UserRole Role { get; set; }
}