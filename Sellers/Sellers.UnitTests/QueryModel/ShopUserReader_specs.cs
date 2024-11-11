using FluentAssertions;

namespace Sellers.QueryModel;

public class ShopUserReader_specs
{
    [Theory, AutoSellersData]
    public async Task Sut_returns_user_entity_with_matching_name(
        Func<SellersDbContext> contxtFactory,
        Shop shop,
        ShopUserReader sut)
    {
        using SellersDbContext dbContext = contxtFactory();
        dbContext.Shops.Add(shop);
        await dbContext.SaveChangesAsync();

        User? actual = await sut.FindUser(username: shop.UserId!);

        actual.Should().NotBeNull();
        actual!.Username.Should().Be(shop.UserId);
        actual.PasswordHash.Should().Be(shop.PasswordHash);
    }

    [Theory, AutoSellersData]
    public async Task Sut_sets_user_id_with_shop_id(
        Func<SellersDbContext> contextFactory,
        Shop shop,
        ShopUserReader sut)
    {
        using SellersDbContext dbContext = contextFactory();
        dbContext.Shops.Add(shop);
        await dbContext.SaveChangesAsync();

        User? actual = await sut.FindUser(username: shop.UserId!);

        actual!.Id.Should().Be(shop.Id);
    }

    [Theory, AutoSellersData]
    public async Task Sut_returns_null_with_nonexistent_username(
        ShopUserReader sut,
        string username)
    {
        User? actual = await sut.FindUser(username);
        actual.Should().BeNull();
    }
}