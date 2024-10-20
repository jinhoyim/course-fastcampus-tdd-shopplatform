using Microsoft.EntityFrameworkCore;
using Orders.Domain;
using Orders.Domain.Exception;
using Orders.Domain.Model;
using Orders.Infrastructure;

namespace Orders.Api;

public class OrderService(OrdersDbContext dbContext)
{
    public OrdersDbContext DbContext { get; init; } = dbContext;

    public async Task<List<Order>> GetOrders(Guid? userId)
    {
        if (userId == null || userId.Equals(Guid.Empty))
        {
            return await DbContext.Orders.AsNoTracking().ToListAsync();
        }
        else
        {
            return await DbContext.Orders.AsNoTracking()
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }
    }

    public async Task<Order> PlaceOrder(Guid userId, Guid shopId, Guid itemId, decimal price)
    {
        if (price <= 0)
        {
            throw new InvalidOrderException("Price must be greater than zero.");
        }
        var order = Order.Create(userId, shopId, itemId, price);
        await DbContext.Orders.AddAsync(order);
        await DbContext.SaveChangesAsync();
        return order;
    }

    public async Task<Order> GetOrderById(Guid orderId)
    {
        var order = await DbContext.Orders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == orderId);
        if (order == null)
        {
            throw new OrderNotFoundException($"Order with id {orderId} not found");
        }
        return order;
    }

    private async Task<Order> FindOrderById(Guid orderId)
    {
        var order = await DbContext.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
        if (order == null)
        {
            throw new OrderNotFoundException($"Order with id {orderId} not found");
        }
        return order;
    }

    public async Task<Order> StartOrder(Guid orderId)
    {
        var order = await FindOrderById(orderId);
        order.StartOrder();
        await DbContext.SaveChangesAsync();
        return order;
    }

    public async Task<Order> PaymentCompleted(Guid orderId, DateTime eventTimeUtc)
    {
        var order = await FindOrderById(orderId);
        order.PaymentCompleted(eventTimeUtc);
        await DbContext.SaveChangesAsync();
        return order;
    }

    public async Task<Order> ItemShipped(Guid orderId, DateTime eventTimeUtc)
    {
        var order = await FindOrderById(orderId);
        order.ItemShipped(eventTimeUtc);
        await DbContext.SaveChangesAsync();
        return order;
    }
}