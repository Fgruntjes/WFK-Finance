namespace App.Lib.ServiceBus;

public class ServiceBusOptions
{
    public const string ConnectionStringName = Section;
    public const string Section = "ServiceBus";

    public string Queue { get; set; } = null!;
}