namespace App.Lib.ServiceBus.Messages;

[AttributeUsage(AttributeTargets.Class)]
public class MessageQueueAttribute : Attribute
{
    public string QueueName { get; }

    public MessageQueueAttribute(string queueName)
    {
        QueueName = queueName;
    }
}