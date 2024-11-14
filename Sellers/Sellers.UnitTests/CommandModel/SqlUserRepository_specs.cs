using System.Collections.Immutable;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Sellers.CommandModel;

public class SqlUserRepository_specs
{
    [Theory, AutoSellersData]
    public async Task Sut_correctly_adds_entity(
        SqlUserRepository sut,
        User source,
        Func<SellersDbContext> contextFactory)
    {
        User user = source with { Roles = ImmutableArray<Role>.Empty };
        await sut.Add(user);
        using SellersDbContext dbContext = contextFactory();
        
        UserEntity? actual = await dbContext.Users.SingleOrDefaultAsync(x => x.Id == user.Id);
        
        actual.Should().BeEquivalentTo(user, c => c.ExcludingMissingMembers());
    }

    [Theory, AutoSellersData]
    public async Task Sut_returns_true_if_user_exists(
        Func<SellersDbContext> contextFactory,
        SqlUserRepository sut,
        User user)
    {
        await sut.Add(user);
        bool actual = await sut.TryUpdate(user.Id, x => x);
        actual.Should().BeTrue();
    }

    [Theory, AutoSellersData]
    public async Task Sut_returns_false_if_user_not_exists(
        SqlUserRepository sut,
        Guid userId)
    {
        bool actual = await sut.TryUpdate(userId, x => x);
        actual.Should().BeFalse();
    }
}