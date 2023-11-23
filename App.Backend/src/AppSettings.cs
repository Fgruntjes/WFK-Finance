namespace App.Backend;

public class AppSettings
{
    public const string App = "App";
    public string Environment { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string FrontendUrl { get; set; } = string.Empty;
}