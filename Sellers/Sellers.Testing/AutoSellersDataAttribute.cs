using AutoFixture;
using AutoFixture.Xunit2;

namespace Sellers;

public sealed class AutoSellersDataAttribute()
    : AutoDataAttribute(() => new Fixture().Customize(
        new CompositeCustomization(
            new ShopCustomization(),
            new PasswordHasherCustomization(),
            new SellersServerCustomization()
        )));