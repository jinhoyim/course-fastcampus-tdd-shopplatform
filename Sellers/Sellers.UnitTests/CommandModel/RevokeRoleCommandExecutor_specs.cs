using AutoFixture.Xunit2;
using FluentAssertions;
using Sellers.Commands;
using Sellers.QueryModel;

namespace Sellers.CommandModel;

public class RevokeRoleCommandExecutor_specs
{
    [Theory, AutoSellersData]
    public async Task Sut_correctly_removes_role(
        [Frozen] Func<SellersDbContext> contextFactory,
        UserEntity user,
        Role role,
        RevokeRoleCommandExecutor sut)
    {
        await using SellersDbContext dbContext = contextFactory();
        user.Roles.Add(new RoleEntity { ShopId = role.ShopId, RoleName = role.RoleName });
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
    
        RevokeRole command = new RevokeRole(role.ShopId, role.RoleName);
        await sut.Execute(user.Id, command);
    
        User actual = (await new SqlUserReader(contextFactory).FindUser(user.Id))!;
        actual.Roles.Should().NotContain(role);
    }

    [Theory, AutoSellersData]
    public async Task Sut_removes_only_specified_role(
        [Frozen] Func<SellersDbContext> contextFactory,
        UserEntity user,
        Role role1,
        Role role2,
        RevokeRoleCommandExecutor sut)
    {
        await using SellersDbContext dbContext = contextFactory();
        user.Roles.Add(new RoleEntity { ShopId = role1.ShopId, RoleName = role1.RoleName });
        user.Roles.Add(new RoleEntity { ShopId = role2.ShopId, RoleName = role2.RoleName });
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        await sut.Execute(user.Id, new(role1.ShopId, role1.RoleName));

        User actual = (await new SqlUserReader(contextFactory).FindUser(user.Id))!;
        actual.Roles.Should().Contain(role2);
    }

    [Theory, AutoSellersData]
    public async Task Sut_fails_if_user_not_exists(
        RevokeRoleCommandExecutor sut,
        Guid userId,
        RevokeRole command)
    {
        Func<Task> action = () => sut.Execute(userId, command);
        await action.Should().ThrowAsync<EntityNotFoundException>();
    }
}