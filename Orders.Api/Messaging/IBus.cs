namespace Orders.Api.Messaging;

public interface IBus<T>
{
    Task Send(T message);
}