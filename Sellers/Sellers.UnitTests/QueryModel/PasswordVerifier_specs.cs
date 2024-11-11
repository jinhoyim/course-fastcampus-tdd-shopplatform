using FluentAssertions;

namespace Sellers.QueryModel;

public class PasswordVerifier_specs
{
    [Theory]
    [InlineAutoSellersData("hello world", "hello world", true)]
    [InlineAutoSellersData("hello world", "hello word", false)]
    public async Task VerifyPassword_works_correctly(
        string password,
        string providedPassword,
        bool result,
        Func<SellersDbContext> contextFactory,
        Shop shop,
        ShopUserReader reader,
        AspNetCorePasswordHasher hasher)
    {
        using SellersDbContext dbContext = contextFactory();
        shop.PasswordHash = hasher.HashPassword(password);
        dbContext.Shops.Add(shop);
        await dbContext.SaveChangesAsync();
        PasswordVerifier sut = new(reader, hasher);

        bool actual = await sut.VerifyPassword(shop.UserId!, providedPassword);

        actual.Should().Be(result);
    }

    [Theory, AutoSellersData]
    public async Task VerifyPassword_returns_false_with_nonexistent_username(
        ShopUserReader reader,
        AspNetCorePasswordHasher hasher,
        string username,
        string password)
    {
        PasswordVerifier sut = new(reader, hasher);
        bool actual = await sut.VerifyPassword(username, password);
        actual.Should().BeFalse();
    }
}

