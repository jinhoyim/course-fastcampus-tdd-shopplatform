using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Orders.Api.Commands;
using Orders.Api.Events;
using Orders.Domain.Model;

namespace Orders.UnitTests.api.orders.handle.bank_transfer_payment_completed;

public class Post_specs
{
    [Fact]
    public async Task Sut_returns_BadRequest_if_order_not_started()
    {
        // Arrange
        HttpClient client = OrdersServer.Create().CreateClient();

        PlaceOrder placeOrder = new(
            UserId: Guid.NewGuid(), 
            ShopId: Guid.NewGuid(), 
            ItemId: Guid.NewGuid(),
            Price: 10000);
        var placedResponse = await client.PostAsJsonAsync("api/v1/orders", placeOrder);
        Guid orderId = (await placedResponse.Content.ReadFromJsonAsync<Order>())!.Id;

        BankTransferPaymentCompleted paymentCompleted = new(orderId, EventTimeUtc: DateTime.UtcNow);
        var eventUri = "api/v1/orders/handle/bank-transfer-payment-completed";
        
        // Act
        var response = await client.PostAsJsonAsync(eventUri, paymentCompleted);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Sut_returns_BadRequest_if_payment_already_completed()
    {
        // Arrange
        HttpClient client = OrdersServer.Create().CreateClient();

        PlaceOrder placeOrder = new(
            UserId: Guid.NewGuid(), 
            ShopId: Guid.NewGuid(), 
            ItemId: Guid.NewGuid(),
            Price: 10000);
        var placedResponse = await client.PostAsJsonAsync("api/v1/orders", placeOrder);
        Guid orderId = (await placedResponse.Content.ReadFromJsonAsync<Order>())!.Id;

        StartOrder startOrder = new();
        await client.PostAsJsonAsync($"api/v1/orders/{orderId}/start-order", startOrder);

        BankTransferPaymentCompleted paymentCompleted = new(orderId, EventTimeUtc: DateTime.UtcNow);
        var eventUri = "api/v1/orders/handle/bank-transfer-payment-completed";
        await client.PostAsJsonAsync(eventUri, paymentCompleted);
        
        // Act
        var response = await client.PostAsJsonAsync(eventUri, paymentCompleted);

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
            Price: 10000);
        var placedResponse = await client.PostAsJsonAsync("api/v1/orders", placeOrder);
        Guid orderId = (await placedResponse.Content.ReadFromJsonAsync<Order>())!.Id;

        StartOrder startOrder = new();
        await client.PostAsJsonAsync($"api/v1/orders/{orderId}/start-order", startOrder);

        BankTransferPaymentCompleted paymentCompleted = new(orderId, EventTimeUtc: DateTime.UtcNow);
        var eventUri = "api/v1/orders/handle/bank-transfer-payment-completed";
        await client.PostAsJsonAsync(eventUri, paymentCompleted);
        
        ItemShipped itemShipped = new(orderId, EventTimeUtc: DateTime.UtcNow);
        await client.PostAsJsonAsync("api/v1/orders/handle/item-shipped", itemShipped);
        
        // Act
        var response = await client.PostAsJsonAsync(eventUri, paymentCompleted);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}