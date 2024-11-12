using AutoFixture;

namespace Sellers;

public class SellersDbContextCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        Func<SellersDbContext> factory = SellersDatabase.CreateContext;
        fixture.Register(() => factory);
    }
}