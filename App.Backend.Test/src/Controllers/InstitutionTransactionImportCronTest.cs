namespace App.Backend.Test.Controllers;

public class InstitutionTransactionImportCronTest : IClassFixture<InstitutionAccountTransactionListFixture>
{
    private readonly InstitutionAccountTransactionListFixture _fixture;

    public InstitutionTransactionImportCronTest(InstitutionAccountTransactionListFixture fixture)
    {
        _fixture = fixture;
    }


}