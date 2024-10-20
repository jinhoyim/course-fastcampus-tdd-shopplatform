using System.Net;
using FluentAssertions;

namespace Orders.UnitTests.api.orders.id.start_order;

public class Post_specs
{
    [Fact]
    public async Task Sut_returns_BadRequest_if_order_already_started()
    {
        OrdersServer server = OrdersServer.Create();
        Guid orderId = await server.PlaceOrder();
        await server.StartOrder(orderId);
        
        HttpResponseMessage response = await server.StartOrder(orderId);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Sut_returns_BadRequest_if_payment_completed()
    {
        OrdersServer server = OrdersServer.Create();
        Guid orderId = await server.PlaceOrder();
        await server.StartOrder(orderId);
        await server.HandleBankTransferPaymentCompleted(orderId);
        
        var response = await server.StartOrder(orderId);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Sut_returns_BadRequest_if_order_completed()
    {
        OrdersServer server = OrdersServer.Create();
        Guid orderId = await server.PlaceOrder();
        await server.StartOrder(orderId);
        await server.HandleBankTransferPaymentCompleted(orderId);
        await server.HandleItemShipped(orderId);
        
        var response = await server.StartOrder(orderId);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}