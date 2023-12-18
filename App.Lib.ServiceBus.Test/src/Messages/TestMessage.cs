using App.Lib.ServiceBus.Messages;

namespace App.Lib.ServiceBus.Test.Messages;

public class TestMessage : IMessage
{
    public string Message { get; set; } = string.Empty;
}