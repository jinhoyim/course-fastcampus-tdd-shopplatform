using System.Text.Json.Serialization;
using Orders.Domain.Exception;

namespace Orders.Domain;

public class Order
{
    public Guid Id { get; init; }
    
    [JsonIgnore]
    public long Sequence { get; init; }

    public Guid UserId { get; init; }
    public Guid ShopId { get; init; }
    public Guid ItemId { get; init; }
    public decimal Price { get; init; }
    public OrderStatus Status { get; private set; }
    public DateTime PlacedAtUtc { get; init; }
    public DateTime? StartedAtUtc { get; private set; }
    public DateTime? PaidAtUtc { get; private set; }
    public DateTime? ShippedAtUtc { get; private set; }

    public static Order Create(Guid userId, Guid shopId, Guid itemId, decimal price)
    {
        return new Order()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ShopId = shopId,
            ItemId = itemId,
            Price = price,
            Status = OrderStatus.Pending,
            PlacedAtUtc = DateTime.UtcNow
        };
    }

    public void StartOrder()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new OrderProcessException("Order is not pending");
        }
        Status = OrderStatus.AwaitingPayment;
        StartedAtUtc = DateTime.UtcNow;
    }

    public void PaymentCompleted()
    {
        if (Status != OrderStatus.AwaitingPayment)
        {
            throw new OrderProcessException("Order is not awaiting payment");
        }
        Status = OrderStatus.AwaitingShipment;
        PaidAtUtc = DateTime.UtcNow;
    }

    public void ItemShipped()
    {
        if (Status != OrderStatus.AwaitingShipment)
        {
            throw new OrderProcessException("Order is not awaiting shipment");
        }
        Status = OrderStatus.Completed;
        ShippedAtUtc = DateTime.UtcNow;
    }
}