using App.Lib.ServiceBus.Messages;

namespace App.Lib.ServiceBus;

public interface IMessageHandler<in TMessage>
    where TMessage : IMessage
{
    Task Handle(TMessage message);
}