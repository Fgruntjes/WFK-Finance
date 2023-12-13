namespace App.Lib.ServiceBus.Test.Messages;

public class TestMessageHandler : IMessageHandler<TestMessage>
{
    public IReadOnlyCollection<TestMessage> HandledMessages => _handledMessages.AsReadOnly();

    private readonly IList<TestMessage> _handledMessages = new List<TestMessage>();

    public Task Handle(TestMessage message)
    {
        _handledMessages.Add(message);
        return Task.CompletedTask;
    }
}