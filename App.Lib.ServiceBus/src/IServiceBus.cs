using App.Lib.ServiceBus.Messages;

namespace App.Lib.ServiceBus;

public interface IServiceBus
{
    Task Publish<T>(T message, CancellationToken cancellationToken = default) where T : IMessage;
}