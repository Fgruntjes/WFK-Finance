using App.Backend.Controllers;

namespace App.Backend.Test.Controllers;

public class InstitutionConnectionMutationFixture : AppFixture<InstitutionConnectionMutation>
{

}

public class InstitutionConnectionMutationTest
{
	private readonly InstitutionConnectionMutationFixture _fixture;

	public InstitutionConnectionMutationTest(InstitutionConnectionMutationFixture fixture)
	{
		_fixture = fixture;
	}

}