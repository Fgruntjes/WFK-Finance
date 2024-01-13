using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace App.Lib.Data.Entity;

[PrimaryKey(nameof(UserId), nameof(OrganisationId))]
public class OrganisationUserEntity
{
    public UserEntity User { get; set; } = null!;
    [Key]
    public Guid UserId { get; set; }
    public OrganisationEntity Organisation { get; set; } = null!;
    [Key]
    public Guid OrganisationId { get; set; }
    public UserRole Role { get; set; }
}