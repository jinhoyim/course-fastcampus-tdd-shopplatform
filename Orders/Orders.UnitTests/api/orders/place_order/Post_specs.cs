using System.Net;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Sellers.Api;
using Sellers.UnitTests;

namespace Orders.UnitTests.api.orders.place_order;

public class Post_specs
{
    [Fact]
    public async Task Sut_returns_BadRequest_if_shop_not_exists()
    {
        OrdersServer server = OrdersServer.Create();
        Guid orderId = Guid.NewGuid();
        Guid shopId = Guid.NewGuid();
        HttpResponseMessage response = await server.PlaceOrder(orderId, shopId);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Sut_returns_Ok_if_shop_exists()
    {
        OrdersServer ordersServer = OrdersServer.Create();
        SellersServer sellersServer = ordersServer.Services.GetRequiredService<SellersServer>();

        Guid orderId = Guid.NewGuid();
        HttpResponseMessage response = await ordersServer.PlaceOrder(orderId);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Sut_does_not_create_order_if_shop_not_exists()
    {
        OrdersServer server = OrdersServer.Create();
        Guid orderId = Guid.NewGuid();
        Guid shopId = Guid.NewGuid();
        await server.PlaceOrder(orderId, shopId);

        string uri = $"api/v1/orders/{orderId}";
        HttpResponseMessage response = await server.CreateClient().GetAsync(uri);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}