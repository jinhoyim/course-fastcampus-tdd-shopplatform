using System.Net;
using FluentAssertions;

namespace Orders.api.orders.handle.bank_transfer_payment_completed;

public class Post_specs
{
    [Fact]
    public async Task Sut_fails_if_order_is_pending()
    {
        OrdersServer server = OrdersServer.Create();
        Guid orderId = Guid.NewGuid();
        await server.PlaceOrder(orderId);
        
        var response = await server.HandleBankTransferPaymentCompleted(orderId);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Sut_fails_if_payment_already_completed()
    {
        OrdersServer server = OrdersServer.Create();
        Guid orderId = Guid.NewGuid();
        await server.PlaceOrder(orderId);
        await server.StartOrder(orderId);
        await server.HandleBankTransferPaymentCompleted(orderId);
        
        var response = await server.HandleBankTransferPaymentCompleted(orderId);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Sut_fails_if_order_already_completed()
    {
        OrdersServer server = OrdersServer.Create();
        Guid orderId = Guid.NewGuid();
        await server.PlaceOrder(orderId);
        await server.StartOrder(orderId);
        await server.HandleBankTransferPaymentCompleted(orderId);
        await server.HandleItemShipped(orderId);
        
        var response = await server.HandleBankTransferPaymentCompleted(orderId);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}