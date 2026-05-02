namespace Shared.Messaging.Interfaces;

public interface IMessagePublisher
{
    Task PublishAsync<T>(T message);
}