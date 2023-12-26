namespace App.Lib.InstitutionConnection.Exception;

public sealed class InstitutionAccountNotFoundException : System.Exception
{
    public InstitutionAccountNotFoundException(Guid institutionAccountId)
        : base("InstitutionAccount {InstitutionAccountId} not found")
    {
        Data.Add("InstitutionAccountId", institutionAccountId);
    }
}