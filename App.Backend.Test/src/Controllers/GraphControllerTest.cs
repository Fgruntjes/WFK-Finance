using App.Backend.Test.Fixtures;

namespace App.Backend.Test.Controllers;

public class GraphControllerTest<TDatabaseFixture> : IClassFixture<TDatabaseFixture>
	where TDatabaseFixture : class
{
	protected IGraphQLFixture Fixture { get; }
	
	public GraphControllerTest(IGraphQLFixture fixture)
	{
		Fixture = fixture;
	}
}