using App.Backend.Data;

namespace App.Backend.Startup;

public static class Database
{
	public static void AddDatabase(this IServiceCollection services, string connectionString)
	{
		services.AddNpgsql<DatabaseContext>(connectionString);		
	}
}