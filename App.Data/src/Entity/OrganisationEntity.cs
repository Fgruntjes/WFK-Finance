using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Data.Entity;

public class OrganisationEntity
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public Guid Id { get; set; }
	public string Slug { get; set; } = null!;
	public ICollection<OrganisationUserEntity> Users { get; } = new List<OrganisationUserEntity>();
	public ICollection<InstitutionConnectionEntity> InstitutionConnections { get; } = new List<InstitutionConnectionEntity>();

	public OrganisationEntity()
	{
		Id = Guid.NewGuid();
	}
}