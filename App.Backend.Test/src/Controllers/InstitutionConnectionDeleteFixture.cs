using App.Backend.Controllers;
using App.Data.Entity;
using Xunit.Abstractions;

namespace App.Backend.Test.Controllers;

public class InstitutionConnectionDeleteFixture : AppFixture<InstitutionConnectionDeleteController>
{
    public Guid InstitutionConnectionId { get; internal set; }

    public InstitutionConnectionDeleteFixture(IMessageSink logMessageSink)
        : base(logMessageSink)
    {
        InstitutionConnectionId = new Guid("56612691-dcf7-44e2-b506-ba83b60de5a9");
        var institutionConnectionEntity = new InstitutionConnectionEntity
        {
            Id = InstitutionConnectionId,
            OrganisationId = OrganisationId,
            ConnectUrl = new Uri("https://www.example.com/connect-url/refresh"),
            ExternalId = "ed69f988-a1fb-4e89-8d56-66b42e43a675"
        };
        Database.InstitutionConnections.Add(institutionConnectionEntity);

        Database.SaveChanges();
    }
}
