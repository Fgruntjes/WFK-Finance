using Microsoft.EntityFrameworkCore;

namespace App.Backend.Data.Entity;

[PrimaryKey(nameof(UserId), nameof(OrganisationId))]
public class OrganisationUserEntity
{
	public UserEntity User { get; set; } = null!;
	public Guid UserId {get; set;}
	public OrganisationEntity Organisation { get; set; } = null!;
	public Guid OrganisationId {get; set;}
	public UserRole Role { get; set; }
}