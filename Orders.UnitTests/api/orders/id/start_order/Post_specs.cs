using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Orders.Api.Commands;
using Orders.Api.Events;
using Orders.Domain.Model;

namespace Orders.UnitTests.api.orders.id.start_order;

public class Post_specs
{
    [Fact]
    public async Task Sut_returns_BadRequest_if_order_already_started()
    {
        // Arrange
        OrdersServer server = OrdersServer.Create();
        HttpClient client = server.CreateClient();
        
        Guid orderId = await server.PlaceOrder();

        StartOrder startOrder = new();
        var startOrderUri = $"api/v1/orders/{orderId}/start-order";
        await client.PostAsJsonAsync(startOrderUri, startOrder);
        
        // Act
        var response = await client.PostAsJsonAsync(startOrderUri, startOrder);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Sut_returns_BadRequest_if_payment_completed()
    {
        // Arrange
        HttpClient client = OrdersServer.Create().CreateClient();

        PlaceOrder placeOrder = new(
            UserId: Guid.NewGuid(),
            ShopId: Guid.NewGuid(),
            ItemId: Guid.NewGuid(),
            Price: 11000);
        var placedOrderResponse = await client.PostAsJsonAsync("api/v1/orders", placeOrder);
        Guid orderId = (await placedOrderResponse.Content.ReadFromJsonAsync<Order>())!.Id;

        StartOrder startOrder = new();
        var startOrderUri = $"api/v1/orders/{orderId}/start-order";
        await client.PostAsJsonAsync(startOrderUri, startOrder);
        
        BankTransferPaymentCompleted paymentCompleted = new(orderId, EventTimeUtc: DateTime.UtcNow);
        await client.PostAsJsonAsync("api/v1/orders/handle/bank-transfer-payment-completed", paymentCompleted);
        
        // Act
        var response = await client.PostAsJsonAsync(startOrderUri, startOrder);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Sut_returns_BadRequest_if_order_completed()
    {
        // Arrange
        HttpClient client = OrdersServer.Create().CreateClient();

        PlaceOrder placeOrder = new(
            UserId: Guid.NewGuid(),
            ShopId: Guid.NewGuid(),
            ItemId: Guid.NewGuid(),
            Price: 11000);
        var placedOrderResponse = await client.PostAsJsonAsync("api/v1/orders", placeOrder);
        Guid orderId = (await placedOrderResponse.Content.ReadFromJsonAsync<Order>())!.Id;

        StartOrder startOrder = new();
        var startOrderUri = $"api/v1/orders/{orderId}/start-order";
        await client.PostAsJsonAsync(startOrderUri, startOrder);
        
        BankTransferPaymentCompleted paymentCompleted = new(orderId, EventTimeUtc: DateTime.UtcNow);
        await client.PostAsJsonAsync("api/v1/orders/handle/bank-transfer-payment-completed", paymentCompleted);
        
        ItemShipped itemShipped = new(orderId, EventTimeUtc: DateTime.UtcNow);
        await client.PostAsJsonAsync("api/v1/orders/handle/item-shipped", itemShipped);
        
        // Act
        var response = await client.PostAsJsonAsync(startOrderUri, startOrder);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}