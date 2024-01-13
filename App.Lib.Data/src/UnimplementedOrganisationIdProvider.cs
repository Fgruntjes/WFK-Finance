namespace App.Lib.Data;

public class UnimplementedOrganisationIdProvider : IOrganisationIdProvider
{
    public Guid GetOrganisationId()
    {
        throw new NotImplementedException();
    }
}