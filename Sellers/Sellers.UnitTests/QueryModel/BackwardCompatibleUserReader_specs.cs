using FluentAssertions;

namespace Sellers.QueryModel;

public class BackwardCompatibleUserReader_specs
{
    [Theory, AutoSellersData]
    public async Task Sut_returns_correct_entity_with_username_from_user_data_model(
        Func<SellersDbContext> contextFactory,
        UserEntity user,
        BackwardCompatibleUserReader sut)
    {
        await using SellersDbContext dbContext = contextFactory();
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        User? actual = await sut.FindUser(user.Username);
        
        actual.Should().BeEquivalentTo(user, c => c.ExcludingMissingMembers());
    }

    [Theory, AutoSellersData]
    public async Task Sut_returns_correct_entity_from_shop_data_model(
        Func<SellersDbContext> contextFactory,
        Shop shop,
        BackwardCompatibleUserReader sut)
    {
        await using SellersDbContext dbContext = contextFactory();
        dbContext.Shops.Add(shop);
        await dbContext.SaveChangesAsync();

        User? actual = await sut.FindUser(shop.UserId);

        actual.Should().BeEquivalentTo(
            await new ShopUserReader(contextFactory).FindUser(shop.UserId));
    }
    
    [Theory, AutoSellersData]
    public async Task Sut_returns_correct_entity_with_id_from_user_data_model(
        Func<SellersDbContext> contextFactory,
        UserEntity user,
        BackwardCompatibleUserReader sut)
    {
        await using SellersDbContext dbContext = contextFactory();
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        User? actual = await sut.FindUser(user.Id);
        
        actual.Should().BeEquivalentTo(user, c => c.ExcludingMissingMembers());
    }
    
    [Theory, AutoSellersData]
    public async Task Sut_returns_correct_entity_with_id_from_shop_data_model(
        Func<SellersDbContext> contextFactory,
        Shop shop,
        BackwardCompatibleUserReader sut)
    {
        await using SellersDbContext dbContext = contextFactory();
        dbContext.Shops.Add(shop);
        await dbContext.SaveChangesAsync();

        User? actual = await sut.FindUser(shop.Id);

        actual.Should().BeEquivalentTo(
            await new ShopUserReader(contextFactory).FindUser(shop.Id));
    }
}