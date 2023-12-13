using App.Lib.ServiceBus.Messages;
using Rebus.Handlers;

namespace App.Lib.ServiceBus;

public class RebusHandler<TMessage, TMessageHandler> : IHandleMessages<TMessage>
    where TMessageHandler : IMessageHandler<TMessage>
    where TMessage : IMessage
{
    private readonly TMessageHandler _handler;

    public RebusHandler(TMessageHandler handler)
    {
        _handler = handler;
    }

    public Task Handle(TMessage message)
    {
        return _handler.Handle(message);
    }
}