using System.Net.Http.Json;
using FluentAssertions;
using Orders.Application.Commands;
using Orders.Domain.Model;
using Sellers;
using Sellers.UnitTests;

namespace Orders.UnitTests.api.orders;

public class Get_specs
{
    [Fact]
    public async Task Sut_correctly_applies_user_filter()
    {
        // Arrange
        OrdersServer server = OrdersServer.Create();
        using var client = server.CreateClient();
        Shop shop = await server.GetSellersServer().CreateShop();
        
        Guid shopId = shop.Id;
        Guid itemId = Guid.NewGuid();
        decimal price = 100000;

        List<PlaceOrder> commands =
        [
            new(UserId: Guid.NewGuid(), shopId, itemId, price),
            new(UserId: Guid.NewGuid(), shopId, itemId, price),
            new(UserId: Guid.NewGuid(), shopId, itemId, price)
        ];

        await Task.WhenAll(from command in commands
            let uri = $"api/v1/orders/{Guid.NewGuid()}/place-order"
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
        OrdersServer server = OrdersServer.Create();
        using var client = server.CreateClient();

        Guid userId = Guid.NewGuid();
        Guid itemId = Guid.NewGuid();
        decimal price = 100000;

        SellersServer sellersServer = server.GetSellersServer();
        async Task<Guid> GetShopId() => (await sellersServer.CreateShop()).Id;

        List<PlaceOrder> commands =
        [
            new(userId, ShopId: await GetShopId(), itemId, price),
            new(userId, ShopId: await GetShopId(), itemId, price),
            new(userId, ShopId: await GetShopId(), itemId, price)
        ];

        await Task.WhenAll(from command in commands
            let uri = $"api/v1/orders/{Guid.NewGuid()}/place-order"
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
        OrdersServer server = OrdersServer.Create();
        using var client = server.CreateClient();
        SellersServer sellersServer = server.GetSellersServer();
        async Task<Guid> GetShopId() => (await sellersServer.CreateShop()).Id;

        Guid userId = Guid.NewGuid();
        Guid shopId = await GetShopId();

        List<PlaceOrder> commands =
        [
            new(userId, ShopId: await GetShopId(), ItemId: Guid.NewGuid(), Price: 100),
            new(userId, shopId, ItemId: Guid.NewGuid(), Price: 100),
            new(UserId: Guid.NewGuid(), shopId, ItemId: Guid.NewGuid(), Price: 100),
        ];

        await Task.WhenAll(from command in commands
            let uri = $"api/v1/orders/{Guid.NewGuid()}/place-order"
            select client.PostAsJsonAsync(uri, command));
        
        // Act
        string queryUri = $"api/v1/orders?user-id={userId}&shop-id={shopId}";
        var response = await client.GetAsync(queryUri);
        Order[]? actual = await response.Content.ReadFromJsonAsync<Order[]>();
        
        // Assert
        actual.Should().OnlyContain(x => x.UserId == userId && x.ShopId == shopId);
    }
}