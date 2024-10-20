using System.Net;
using FluentAssertions;

namespace Orders.UnitTests.api.orders.handle.bank_transfer_payment_completed;

public class Post_specs
{
    [Fact]
    public async Task Sut_fails_if_order_is_pending()
    {
        OrdersServer server = OrdersServer.Create();
        Guid orderId = await server.PlaceOrder();
        
        var response = await server.HandleBankTransferPaymentCompleted(orderId);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Sut_fails_if_payment_already_completed()
    {
        OrdersServer server = OrdersServer.Create();
        HttpClient client = server.CreateClient();
        Guid orderId = await server.PlaceOrder();
        await server.StartOrder(orderId);
        await server.HandleBankTransferPaymentCompleted(orderId);
        
        var response = await server.HandleBankTransferPaymentCompleted(orderId);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Sut_fails_if_order_already_completed()
    {
        OrdersServer server = OrdersServer.Create();
        HttpClient client = server.CreateClient();
        Guid orderId = await server.PlaceOrder();
        await server.StartOrder(orderId);
        await server.HandleBankTransferPaymentCompleted(orderId);
        await server.HandleItemShipped(orderId);
        
        var response = await server.HandleBankTransferPaymentCompleted(orderId);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}