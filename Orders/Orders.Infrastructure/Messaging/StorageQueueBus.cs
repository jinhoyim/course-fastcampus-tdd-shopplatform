using System.Reactive.Disposables;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Orders.Events;

namespace Orders.Messaging;

public class StorageQueueBus :
    IBus<PaymentApproved>,
    IAsyncObservable<PaymentApproved>
{
    private readonly QueueClient _client;

    public StorageQueueBus(QueueClient client)
    {
        _client = client;
    }

    public Task Send(PaymentApproved message)
        => _client.SendMessageAsync(BinaryData.FromObjectAsJson(message));

    public IDisposable Subscribe(Func<PaymentApproved, Task> onNext)
    {
        bool listening = true;
        Run();
        return Disposable.Create(() =>
        {
            listening = false;
        });

        async void Run()
        {
            await _client.CreateIfNotExistsAsync();
            while (listening)
            {
                QueueMessage[] messages = await _client.ReceiveMessagesAsync();
                foreach (QueueMessage message in messages)
                {
                    if (listening)
                    {
                        PaymentApproved? paymentApproved = message.Body.ToObjectFromJson<PaymentApproved>();
                        if (paymentApproved != null)
                            await onNext.Invoke(paymentApproved);
                        await _client.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                    }
                }
            }
        }
    }
}