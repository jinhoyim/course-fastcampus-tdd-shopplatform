using AutoFixture;
using AutoFixture.Xunit2;

namespace Sellers;

public sealed class AutoSellersDataAttribute()
    : AutoDataAttribute(() => new Fixture().Customize(
        new CompositeCustomization(
            new ShopCustomization(),
            new UserCustomization(),
            new RoleCustomization(),
            new PasswordHasherCustomization(),
            new SellersDbContextCustomization(),
            new UserRepositoryCustomization(),
            new SellersServerCustomization()
        )));