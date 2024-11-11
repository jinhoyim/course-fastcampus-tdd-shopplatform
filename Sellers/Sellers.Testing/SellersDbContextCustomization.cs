using AutoFixture;

namespace Sellers;

public class SellersDbContextCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Register(() => SellersDatabase.GetContext());
    }
}