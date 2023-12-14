using App.Lib.ServiceBus.Messages;
using Rebus.Handlers;

namespace App.Lib.ServiceBus;

public class RebusHandler<TMessage, TMessageHandler> : IHandleMessages<TMessage>
    where TMessageHandler : IMessageHandler<TMessage>
    where TMessage : IMessage
{
    private readonly TMessageHandler _handler;
    private readonly ApplicationIdleService _idleService;


    public RebusHandler(TMessageHandler handler, ApplicationIdleService idleService)
    {
        _handler = handler;
        _idleService = idleService;
    }

    public async Task Handle(TMessage message)
    {
        try
        {
            await _handler.Handle(message);
        }
        finally
        {
            _idleService.ResetExitTimer();
        }
    }
}