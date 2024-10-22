using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Orders.Application.Events;
using Orders.Domain.Model;

namespace Orders.UnitTests.api.orders.handle.item_shipped;

public class Post_specs
{
    [Fact]
    public async Task Sut_correctly_sets_event_time()
    {
        // Arrange
        OrdersServer server = OrdersServer.Create();
        using var client = server.CreateClient();
        Guid orderId = await server.PlaceOrder();
        await server.StartOrder(orderId);
        await server.HandleBankTransferPaymentCompleted(orderId);
        
        // Act
        var shippedEventTimeUtc = DateTime.UtcNow;
        ItemShipped itemShipped = new(orderId, EventTimeUtc: shippedEventTimeUtc);
        await client.PostAsJsonAsync("api/v1/orders/handle/item-shipped", itemShipped);
        
        // Assert
        Order? order = await client.GetFromJsonAsync<Order>($"api/v1/orders/{orderId}");
        // 데이터베이스에 저장할 때 시간 오차가 발생할 수 있기 때문에 BeCloseTo 로 근사한 값을 검증한다.
        order!.ShippedAtUtc.Should().BeCloseTo(shippedEventTimeUtc, TimeSpan.FromMilliseconds(10));
    }

    [Fact]
    public async Task Sut_fails_if_order_not_started()
    {
        OrdersServer server = OrdersServer.Create();
        Guid orderId = await server.PlaceOrder();

        HttpResponseMessage response = await server.HandleItemShipped(orderId);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Sut_fails_if_payment_not_completed()
    {
        OrdersServer server = OrdersServer.Create();
        Guid orderId = await server.PlaceOrder();
        await server.StartOrder(orderId);

        HttpResponseMessage response = await server.HandleItemShipped(orderId);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Sut_fails_if_order_already_completed()
    {
        OrdersServer server = OrdersServer.Create();
        Guid orderId = await server.PlaceOrder();
        await server.StartOrder(orderId);
        await server.HandleBankTransferPaymentCompleted(orderId);
        await server.HandleItemShipped(orderId);

        HttpResponseMessage response = await server.HandleItemShipped(orderId);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}