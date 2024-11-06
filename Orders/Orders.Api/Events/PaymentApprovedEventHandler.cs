using Microsoft.EntityFrameworkCore;
using Orders.Messaging;
using Orders.Model;

namespace Orders.Events;

public static class PaymentApprovedEventHandler
{
    public static void Listen(IServiceProvider services)
    {
        var observable = services.GetRequiredService<IAsyncObservable<PaymentApproved>>();
        
        observable.Subscribe(async listenedEvent =>
        {
            using var scope = services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
            
            IQueryable<Order> query =
                from x in dbContext.Orders
                where x.PaymentTransactionId == listenedEvent.PaymentTransactionId
                select x;
            
            if (await query.SingleOrDefaultAsync() is { } order)
            {
                order.PaymentCompleted(listenedEvent.EventTimeUtc);
                await dbContext.SaveChangesAsync();
            }
        });
    }
}