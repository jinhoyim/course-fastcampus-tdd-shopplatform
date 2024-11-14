using AutoFixture.Xunit2;
using FluentAssertions;
using Sellers.Commands;
using Sellers.QueryModel;

namespace Sellers.CommandModel;

public class GrantRoleCommandExecutor_specs
{
    [Theory, AutoSellersData]
    public async Task Sut_correctly_add_role(
        [Frozen] Func<SellersDbContext> contextFactory,
        UserEntity user,
        GrantRoleCommandExecutor sut,
        GrantRole command)
    {
        await using SellersDbContext dbContext = contextFactory();
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        await sut.Execute(user.Id, command);

        User? actual = (await new SqlUserReader(contextFactory).FindUser(user.Id))!;
        actual.Roles.Should().Contain(new Role(command.ShopId, command.RoleName));
    }
    
    [Theory, AutoSellersData]
    public async Task Sut_correctly_appends_role(
        [Frozen] Func<SellersDbContext> contextFactory,
        UserEntity user,
        GrantRoleCommandExecutor sut,
        GrantRole command1,
        GrantRole command2)
    {
        await using SellersDbContext dbContext = contextFactory();
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        await sut.Execute(user.Id, command1);
        
        await sut.Execute(user.Id, command2);
    
        User actual = (await new SqlUserReader(contextFactory).FindUser(user.Id))!;
        actual.Roles.Should().Contain(new Role(command1.ShopId, command1.RoleName));
        actual.Roles.Should().Contain(new Role(command2.ShopId, command2.RoleName));
    }

    [Theory, AutoSellersData]
    public async Task Sut_correctly_appends_role_InMemory(
        InMemoryUserRepository repository,
        User user,
        GrantRole command1,
        GrantRole command2)
    {
        GrantRoleCommandExecutor sut = new(repository);
        await repository.Add(user);
        await sut.Execute(user.Id, command1);
        
        await sut.Execute(user.Id, command2);

        User actual = repository.Single(x => x.Id == user.Id);
        actual.Roles.Should().Contain(new Role(command1.ShopId, command1.RoleName));
        actual.Roles.Should().Contain(new Role(command2.ShopId, command2.RoleName));
    }

    [Theory, AutoSellersData]
    public async Task Sut_fails_if_user_not_exists(
        GrantRoleCommandExecutor sut,
        Guid id,
        GrantRole command)
    {
        Func<Task> action = () => sut.Execute(id, command);
        await action.Should().ThrowAsync<EntityNotFoundException>();
    }
    
    public class InMemoryUserRepository : List<User>, IUserRepository
    {
        public Task Add(User user)
        {
            base.Add(user);
            return Task.CompletedTask;
        }

        public Task<bool> TryUpdate(Guid id, Func<User, User> reviser)
        {
            if (this.SingleOrDefault(x => x.Id == id) is { } user)
            {
                int index = FindIndex(x => x == user);
                this[index] = reviser(user);
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }
    }
}