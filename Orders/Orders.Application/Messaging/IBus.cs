namespace Orders.Application.Messaging;

public interface IBus<T>
{
    Task Send(T message);
}