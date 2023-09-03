using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Backend.Data.Entity;

public class InstitutionConnectionEntity
{
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public Guid Id { get; set; }
	
	public string ExternalId { get; set; } = null!;
	
	public string ConnectUrl { get; set; } = null!;
    
	public OrganisationEntity Organisation { get; set; } = null!;
    
	[Required]
	public Guid OrganisationId { get; set; }
    
	public InstitutionEntity Institution { get; set; } = null!;
    
	[Required]
	public Guid InstitutionId { get; set; }

	public ICollection<InstitutionConnectionAccountEntity> Accounts { get; } =
		new List<InstitutionConnectionAccountEntity>();
}