using App.Lib.Test.Database;
using Microsoft.Extensions.Logging;
using App.Lib.Test;

namespace App.Backend.Test;

public class AppFixture : FunctionalTestFixture
{
    public IServiceProvider Services { get; }
    public HttpClient Client { get; }

    public AppFixture(DatabasePool databasePool, ILoggerProvider loggerProvider)
        : base(databasePool)
    {
        var application = new ApplicationFactory(Database, loggerProvider);
        Services = application.Services;
        Client = application.CreateClient();
    }
}