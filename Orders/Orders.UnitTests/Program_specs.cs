using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Orders.Api;

namespace Orders.UnitTests;

public class Program_specs
{
    [Fact]
    public void Sut_correctly_registers_SellersService()
    {
        OrdersServer server = OrdersServer.Create();
        SellersService? actual = server.Services.GetService<SellersService>();
        actual.Should().NotBeNull();
    }
}