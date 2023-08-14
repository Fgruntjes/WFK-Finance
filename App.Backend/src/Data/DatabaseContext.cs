using App.Backend.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace App.Backend.Data;

public class DatabaseContext : DbContext
{
	public DbSet<InstitutionEntity> Institutions { get; set; } = null!;
	
	public DatabaseContext(DbContextOptions options) : base(options)
	{
	}
}