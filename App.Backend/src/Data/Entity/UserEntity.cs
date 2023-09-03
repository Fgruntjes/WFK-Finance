using System.ComponentModel.DataAnnotations.Schema;

namespace App.Backend.Data.Entity;

public class UserEntity
{
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public Guid Id { get; set; }
	public string ExternalId { get; set; } = null!;
	public ICollection<OrganisationUserEntity> Organisations { get; } = new List<OrganisationUserEntity>();
}