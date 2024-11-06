using Orders.Exception;
using Orders.Model;
using Orders.Model.Specifications;

namespace Orders;

public class OrderService(IOrderRepository repository)
{
    private readonly IOrderRepository _repository = repository;

    public async Task<IEnumerable<Order>> GetOrders(Guid? userId, Guid? shopId)
    {
        List<ISpecification<Order>> specifications =
        [
            FilterByUserSpecification.Create(userId),
            FilterByShopSpecification.Create(shopId),
        ];
        
        return await _repository.GetAllOrders(specifications, trackChanges: false);
    }

    public async Task<Order> PlaceOrder(Guid orderId, Guid userId, Guid shopId, Guid itemId, decimal price)
    {
        if (price <= 0)
        {
            throw new InvalidOrderException("Price must be greater than zero.");
        }
        var order = Order.Create(orderId, userId, shopId, itemId, price);
        await _repository.Add(order);
        await _repository.UnitOfWork.SaveChangesAsync();
        return order;
    }

    public async Task<Order> GetOrderById(Guid orderId)
    {
        var order = await _repository.FindOrderById(orderId, trackChanges: false);
        if (order == null)
        {
            throw new OrderNotFoundException($"Order with id {orderId} not found");
        }
        return order;
    }

    private async Task<Order> FindOrderById(Guid orderId)
    {
        var order = await _repository.FindOrderById(orderId, trackChanges: true);
        if (order == null)
        {
            throw new OrderNotFoundException($"Order with id {orderId} not found");
        }
        return order;
    }

    public async Task<Order> StartOrder(Guid orderId, string? paymentTransactionId)
    {
        var order = await FindOrderById(orderId);
        order.StartOrder(paymentTransactionId);
        await _repository.UnitOfWork.SaveChangesAsync();
        return order;
    }

    public async Task<Order> PaymentCompleted(Guid orderId, DateTime eventTimeUtc)
    {
        var order = await FindOrderById(orderId);
        order.PaymentCompleted(eventTimeUtc);
        await _repository.UnitOfWork.SaveChangesAsync();
        return order;
    }

    public async Task<Order> ItemShipped(Guid orderId, DateTime eventTimeUtc)
    {
        var order = await FindOrderById(orderId);
        order.ItemShipped(eventTimeUtc);
        await _repository.UnitOfWork.SaveChangesAsync();
        return order;
    }
}