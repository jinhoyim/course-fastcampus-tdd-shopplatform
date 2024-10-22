using System.Net.Http.Json;
using FluentAssertions;
using Orders.Application.Commands;
using Orders.Domain.Model;

namespace Orders.UnitTests.api.orders;

public class Get_specs
{
    [Fact]
    public async Task Sut_correctly_applies_user_filter()
    {
        // Arrange
        using var client = OrdersServer.Create().CreateClient();

        Guid shopId = Guid.NewGuid();
        Guid itemId = Guid.NewGuid();
        decimal price = 100000;

        List<PlaceOrder> commands =
        [
            new(UserId: Guid.NewGuid(), shopId, itemId, price),
            new(UserId: Guid.NewGuid(), shopId, itemId, price),
            new(UserId: Guid.NewGuid(), shopId, itemId, price)
        ];

        await Task.WhenAll(from command in commands
            let uri = "api/v1/orders"
            select client.PostAsJsonAsync(uri, command));

        Guid userId = commands[0].UserId;
        
        // Act
        string queryUri = $"api/v1/orders?user-id={userId}";
        var response = await client.GetAsync(queryUri);
        Order[]? actual = await response.Content.ReadFromJsonAsync<Order[]>();
        
        // Assert
        actual.Should().OnlyContain(x => x.UserId == userId);
    }

    [Fact]
    public async Task Sut_correctly_filters_orders_by_shop()
    {
        // Arrange
        using var client = OrdersServer.Create().CreateClient();

        Guid userId = Guid.NewGuid();
        Guid itemId = Guid.NewGuid();
        decimal price = 100000;

        List<PlaceOrder> commands =
        [
            new(userId, ShopId: Guid.NewGuid(), itemId, price),
            new(userId, ShopId: Guid.NewGuid(), itemId, price),
            new(userId, ShopId: Guid.NewGuid(), itemId, price)
        ];

        await Task.WhenAll(from command in commands
            let uri = "api/v1/orders"
            select client.PostAsJsonAsync(uri, command));

        Guid shopId = commands[0].ShopId;
        
        // Act
        string queryUri = $"api/v1/orders?shop-id={shopId}";
        var response = await client.GetAsync(queryUri);
        Order[]? actual = await response.Content.ReadFromJsonAsync<Order[]>();
        
        // Assert
        actual.Should().OnlyContain(x => x.ShopId == shopId);
    }

    [Fact]
    public async Task Sut_correctly_filters_orders_by_user_and_shop()
    {
        using var client = OrdersServer.Create().CreateClient();
        
        Guid userId = Guid.NewGuid();
        Guid shopId = Guid.NewGuid();

        List<PlaceOrder> commands =
        [
            new(userId, ShopId: Guid.NewGuid(), ItemId: Guid.NewGuid(), Price: 100),
            new(userId, shopId, ItemId: Guid.NewGuid(), Price: 100),
            new(UserId: Guid.NewGuid(), shopId, ItemId: Guid.NewGuid(), Price: 100),
        ];

        await Task.WhenAll(from command in commands
            let uri = "api/v1/orders"
            select client.PostAsJsonAsync(uri, command));
        
        // Act
        string queryUri = $"api/v1/orders?user-id={userId}&shop-id={shopId}";
        var response = await client.GetAsync(queryUri);
        Order[]? actual = await response.Content.ReadFromJsonAsync<Order[]>();
        
        // Assert
        actual.Should().OnlyContain(x => x.UserId == userId && x.ShopId == shopId);
    }
}