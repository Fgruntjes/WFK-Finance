using App.Lib.ServiceBus.Messages;
using Rebus.Bus;

namespace App.Lib.ServiceBus;

internal class RebusServiceBus : IServiceBus
{
    private readonly IBus _bus;

    public RebusServiceBus(IBus bus)
    {
        _bus = bus;
    }

    public async Task Publish<T>(T message, CancellationToken cancellationToken = default) where T : IMessage
    {
        await _bus.Publish(message);
    }
}