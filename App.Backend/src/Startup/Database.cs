using App.Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace App.Backend.Startup;

public static class Database
{
	public static void AddDatabase(this IServiceCollection services, string connectionString)
	{
		services.AddNpgsql<DatabaseContext>(connectionString, o => o.UseNodaTime());
	}
}