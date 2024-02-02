namespace App.IntegrationTest;

public class TestContext
{
    public string ProjectRoot
    {
        get
        {
            var directory = Directory.GetCurrentDirectory();

            while (directory != null)
            {
                var appSettingsPath = Path.Combine(directory, "appsettings.json");
                if (File.Exists(appSettingsPath))
                {
                    return directory;
                }
                directory = Directory.GetParent(directory)?.FullName;
            }

            throw new Exception("Could not find project root");
        }
    }

    public string GetTestName<TestType>()
    {
        return typeof(TestType).FullName ?? $"unknown-{Guid.NewGuid()}";
    }
}