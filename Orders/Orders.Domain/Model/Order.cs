using System.Text.Json.Serialization;
using Orders.Domain.Exception;

namespace Orders.Domain.Model;

public class Order
{
    public Guid Id { get; init; }
    
    [JsonIgnore]
    public long Sequence { get; init; }

    public Guid UserId { get; init; }
    
    public Guid ShopId { get; init; }
    
    public Guid ItemId { get; init; }
    
    public decimal Price { get; init; }

    [JsonInclude]
    public OrderStatus Status { get; private set; }
    
    [JsonInclude]
    public string? PaymentTransactionId { get; private set; }
    
    public DateTime PlacedAtUtc { get; init; }

    [JsonInclude]
    public DateTime? StartedAtUtc { get; private set; }

    [JsonInclude]
    public DateTime? PaidAtUtc { get; private set; }

    // [JsonIgnore]
    [JsonInclude]
    public DateTime? ShippedAtUtc { get; private set; }

    public static Order Create(Guid orderId, Guid userId, Guid shopId, Guid itemId, decimal price)
    {
        return new Order()
        {
            Id = orderId,
            UserId = userId,
            ShopId = shopId,
            ItemId = itemId,
            Price = price,
            Status = OrderStatus.Pending,
            PlacedAtUtc = DateTime.UtcNow
        };
    }

    public void StartOrder(string? paymentTransactionId)
    {
        if (Status != OrderStatus.Pending)
        {
            throw new OrderProcessException("Order is not pending");
        }
        Status = OrderStatus.AwaitingPayment;
        StartedAtUtc = DateTime.UtcNow;
        PaymentTransactionId = paymentTransactionId;
    }

    public void PaymentCompleted(DateTime eventTimeUtc)
    {
        if (Status != OrderStatus.AwaitingPayment)
        {
            throw new OrderProcessException("Order is not awaiting payment");
        }
        Status = OrderStatus.AwaitingShipment;
        PaidAtUtc = eventTimeUtc;
    }

    public void ItemShipped(DateTime eventTimeUtc)
    {
        if (Status != OrderStatus.AwaitingShipment)
        {
            throw new OrderProcessException("Order is not awaiting shipment");
        }
        Status = OrderStatus.Completed;
        ShippedAtUtc = eventTimeUtc;
    }
}