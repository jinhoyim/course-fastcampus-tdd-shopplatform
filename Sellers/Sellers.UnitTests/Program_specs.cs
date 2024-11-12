using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Sellers.QueryModel;

namespace Sellers;

public class Program_specs
{
    [Theory, AutoSellersData]
    public void Sut_registers_IUserReader_service_with_BackwardCompatibleUserReader(
        SellersServer server)
    {
        IUserReader actual = server.Services.GetRequiredService<IUserReader>();
        actual.Should().BeOfType<BackwardCompatibleUserReader>();
    }
}