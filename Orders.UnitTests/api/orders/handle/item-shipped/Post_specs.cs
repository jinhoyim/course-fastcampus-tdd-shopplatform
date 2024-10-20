using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Orders.Api;
using Orders.Api.Commands;
using Orders.Api.Events;
using Orders.Domain;
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
        
        PlaceOrder placeOrder = new(
            UserId: Guid.NewGuid(),
            ShopId: Guid.NewGuid(),
            ItemId: Guid.NewGuid(),
            Price: 100000);
        var placeOrderResponse = await client.PostAsJsonAsync("api/v1/orders", placeOrder);
        Guid orderId = (await placeOrderResponse.Content.ReadFromJsonAsync<Order>())!.Id;
        
        StartOrder startOrder = new();
        await client.PostAsJsonAsync($"api/v1/orders/{orderId}/start-order", startOrder);
        
        BankTransferPaymentCompleted paymentCompleted = new(orderId, EventTimeUtc: DateTime.UtcNow);
        await client.PostAsJsonAsync("api/v1/orders/handle/bank-transfer-payment-completed", paymentCompleted);
        
        // Act
        var shippedEventTimeUtc = DateTime.UtcNow;
        ItemShipped itemShipped = new(orderId, EventTimeUtc: shippedEventTimeUtc);
        await client.PostAsJsonAsync("api/v1/orders/handle/item-shipped", itemShipped);
        
        // Assert
        Order? order = await client.GetFromJsonAsync<Order>($"api/v1/orders/{orderId}");
        // 데이터베이스에 저장할 때 시간 오차가 발생할 수 있기 때문에 BeCloseTo 로 근사한 값을 검증한다.
        order!.ShippedAtUtc.Should().BeCloseTo(shippedEventTimeUtc, TimeSpan.FromMilliseconds(10));
    }
}