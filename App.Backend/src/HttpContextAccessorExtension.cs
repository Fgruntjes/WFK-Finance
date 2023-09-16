namespace App;

public static class HttpContextAccessorExtension
{
    public static readonly Guid OrganisationGuid = new("ae7113f0-1b52-40e5-9e77-5acb10e7fdad");

    public static Guid GetOrganisationId(this IHttpContextAccessor _)
    {
        // TODO get organisation from claim
        return OrganisationGuid;
    }
}