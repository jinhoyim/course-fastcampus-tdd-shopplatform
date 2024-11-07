using AutoFixture;

namespace Sellers;

public class SellersServerCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Register(() => SellersServer.Create());
    }
}