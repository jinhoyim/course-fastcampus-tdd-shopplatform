using FluentAssertions;

namespace Sellers.QueryModel;

public class SqlUserReader_specs
{
    [Theory, AutoSellersData]
    public async Task Sut_returns_entity_with_matching_name(
        Func<SellersDbContext> contextFactory,
        UserEntity user,
        SqlUserReader sut)
    {
        await using SellersDbContext dbContext = contextFactory();
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        
        User? actual = await sut.FindUser(user.Username);

        actual.Should().BeEquivalentTo(user, c => c.ExcludingMissingMembers());
    }

    [Theory, AutoSellersData]
    public async Task Sut_returns_null_with_nonexistent_name(
        string username,
        SqlUserReader sut)
    {
        User? actual = await sut.FindUser(username);
        actual.Should().BeNull();
    }

    [Theory, AutoSellersData]
    public async Task Sut_returns_user_with_matching_id(
        Func<SellersDbContext> contextFactory,
        SqlUserReader sut,
        UserEntity user)
    {
        await using SellersDbContext dbContext = contextFactory();
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        User? actual = await sut.FindUser(user.Id);

        actual.Should().BeEquivalentTo(user, c => c.ExcludingMissingMembers());
    }
    
    [Theory, AutoSellersData]
    public async Task Sut_returns_null_for_unknown_id(
        SqlUserReader sut,
        Guid id)
    {
        User? actual = await sut.FindUser(id);
        actual.Should().BeNull();
    }
}