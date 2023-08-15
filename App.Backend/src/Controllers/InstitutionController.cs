using App.Backend.Data;
using App.Backend.GraphQL.Type;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using Microsoft.EntityFrameworkCore;

namespace App.Backend.Controllers;

public class InstitutionController : GraphController
{
	private readonly DatabaseContext _database;

	public InstitutionController(DatabaseContext database)
	{
		_database = database;
	}
	
	[QueryRoot("institution")]
	public Task<Institution?> GetInstitution(Guid id)
	{
		return _database.Institutions
			.Where(e => e.Id == id)
			.Select(e => Institution.FromEntity(e))
			.SingleOrDefaultAsync();
	}
}