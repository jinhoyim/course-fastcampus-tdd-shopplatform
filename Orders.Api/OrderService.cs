using Microsoft.EntityFrameworkCore;
using Orders.Domain;
using Orders.Domain.Exception;
using Orders.Infrastructure;

namespace Orders.Api;

public class OrderService(OrdersDbContext dbContext)
{
    public OrdersDbContext DbContext { get; init; } = dbContext;

    public async Task<List<Order>> GetOrders()
    {
        return await DbContext.Orders.AsNoTracking().ToListAsync();
    }

    public async Task<Order> PlaceOrder(Guid userId, Guid shopId, Guid itemId)
    {
        var order = Order.Create(userId, shopId, itemId, 10000);
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

    public async Task<Order> PaymentCompleted(Guid orderId)
    {
        var order = await FindOrderById(orderId);
        order.PaymentCompleted();
        await DbContext.SaveChangesAsync();
        return order;
    }

    public async Task<Order> ItemShipped(Guid orderId)
    {
        var order = await FindOrderById(orderId);
        order.ItemShipped();
        await DbContext.SaveChangesAsync();
        return order;
    }
}