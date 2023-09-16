using App.Backend.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace App.Backend.Data;

public class DatabaseContext : DbContext
{
	public DbSet<CountryEntity> Countries { get; set; } = null!;
	public DbSet<InstitutionEntity> Institutions { get; set; } = null!;
	public DbSet<InstitutionConnectionEntity> InstitutionConnections { get; set; } = null!;
	public DbSet<InstitutionConnectionAccountEntity> InstitutionConnectionAccounts { get; set; } = null!;
	public DbSet<OrganisationEntity> Organisations { get; set; } = null!;
	public DbSet<UserEntity> Users { get; set; } = null!;

	public DatabaseContext(DbContextOptions options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		builder.HasPostgresEnum<UserRole>();

		// Strip entity from table names
		foreach (var entity in builder.Model.GetEntityTypes())
		{
			var newName = entity.GetTableName()
				?.Replace("Entity", "");
			if (newName == null) continue;

			entity.SetTableName(newName);
		}
	}
}