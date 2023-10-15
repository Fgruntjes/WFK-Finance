using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace App.Data;

public static class DatabaseConfiguration
{
	public static void AddDatabase(this IServiceCollection services, string connectionString)
	{
		services.AddSqlServer<DatabaseContext>(connectionString, o => o.UseNodaTime());
	}
}