namespace Orders.Application.Messaging;

public interface IAsyncObservable<T>
{
    IDisposable Subscribe(Func<T, Task> onNext);
}