using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Data.Entity;

public class UserEntity
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public Guid Id { get; set; }
	public string ExternalId { get; set; } = null!;
	public ICollection<OrganisationUserEntity> Organisations { get; } = new List<OrganisationUserEntity>();

	public UserEntity()
	{
		Id = Guid.NewGuid();
	}
}