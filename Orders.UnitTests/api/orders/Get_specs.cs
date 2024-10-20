using System.Net.Http.Json;
using FluentAssertions;
using Orders.Api.Commands;
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
}