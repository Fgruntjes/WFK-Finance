using App.Backend.Controllers;
using App.Backend.Data.Entity;

namespace App.Backend.Test.Controllers;

public class InstitutionControllerTest : GraphControllerTest<InstitutionController>
{
	private static string GET_INSTITUTION_QUERY = @"
		query institution($Id: Guid!) {
			institution(id: $Id) {
				name
			}
		}";
	
	[Fact]
	public async Task ByIdTest_Success()
	{
		var context = CreateQueryContext();
		var addResult = await context.Database.Institutions.AddAsync(new InstitutionEntity()
		{
			Name = "MyFakeName",
			ExternalId = "SomeExternalId"
		});
		await context.Database.SaveChangesAsync();
		
		var result = await context.Render(
			GET_INSTITUTION_QUERY,
			new { addResult.Entity.Id });

		result["data"]?["institution"]?["name"]?
			.AsValue()
			.ToString()
			.Should()
			.Be("MyFakeName");
	}

	[Fact]
	public async Task ByIdTest_NotFound()
	{
		var context = CreateQueryContext();
		var result = await context.Render(
			GET_INSTITUTION_QUERY,
			new { Id = Guid.NewGuid() });

		result["data"]?["institution"]?
			.AsValue()
			.ToString()
			.Should()
			.BeNull();
	}
}