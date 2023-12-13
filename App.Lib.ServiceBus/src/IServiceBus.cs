using App.Lib.ServiceBus.Messages;

namespace App.Lib.ServiceBus;

public interface IServiceBus
{
    Task Send<T>(T message, CancellationToken cancellationToken = default) where T : IMessage;
}