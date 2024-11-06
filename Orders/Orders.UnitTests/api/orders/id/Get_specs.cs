using FluentAssertions;
using Orders.Domain.Model;
using Sellers;

namespace Orders.UnitTests.api.orders.id;

public class Get_specs
{
    [Fact]
    public async Task Sut_correctly_sets_shop_name()
    {
        OrdersServer ordersServer = OrdersServer.Create();
        Guid orderId = Guid.NewGuid();
        await ordersServer.PlaceOrder(orderId);
        
        Order? actual = await ordersServer.FindOrder(orderId);
        
        ShopView shop = await ordersServer.GetSellersServer().GetShop(actual!.ShopId);
        actual.ShopName.Should().Be(shop.Name);
    }
}