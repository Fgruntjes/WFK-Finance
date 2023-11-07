using App.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace App.Data;

public class DatabaseContext : DbContext
{
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
        builder.Entity<OrganisationUserEntity>()
            .Property(e => e.Role)
            .HasConversion(new EnumToStringConverter<UserRole>());

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