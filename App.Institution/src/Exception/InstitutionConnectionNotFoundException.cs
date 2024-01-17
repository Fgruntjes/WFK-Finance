namespace App.Institution.Exception;

public sealed class InstitutionConnectionNotFoundException : System.Exception
{
    public InstitutionConnectionNotFoundException(Guid institutionConnectionId, System.Exception? innerException = null)
        : base("InstitutionConnection {InstitutionConnectionId} not found", innerException)
    {
        Data.Add("InstitutionConnectionId", institutionConnectionId);
    }

    public InstitutionConnectionNotFoundException(string externalConnectionId, System.Exception? innerException = null)
        : base("InstitutionConnection {ExternalConnectionId} not found", innerException)
    {
        Data.Add("ExternalConnectionId", externalConnectionId);
    }
}