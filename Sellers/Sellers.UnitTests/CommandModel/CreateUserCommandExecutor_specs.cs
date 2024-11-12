using FluentAssertions;
using Sellers.Commands;
using Sellers.QueryModel;

namespace Sellers.CommandModel;

public class CreateUserCommandExecutor_specs
{
    [Theory, AutoSellersData]
    public async Task Sut_correctly_creates_entity(
        SqlUserReader reader,
        IPasswordHasher hasher,
        SqlUserRepository repository,
        Guid userId,
        CreateUser command)
    {
        CreateUserCommandExecutor sut = new(reader, hasher, repository);
        
        await sut.Execute(userId, command);

        User? actual = await reader.FindUser(command.Username);
        actual.Should().NotBeNull();
        actual!.Id.Should().Be(userId);
    }

    [Theory, AutoSellersData]
    public async Task Sut_correctly_sets_password_hash(
        SqlUserReader reader,
        IPasswordHasher hasher,
        SqlUserRepository repository,
        Guid userId,
        CreateUser command)
    {
        CreateUserCommandExecutor sut = new(reader, hasher, repository);
        
        await sut.Execute(userId, command);

        User? user = await reader.FindUser(command.Username);
        bool actual = hasher.VerifyPassword(user.PasswordHash, command.Password);
        actual.Should().BeTrue();
    }

    [Theory, AutoSellersData]
    public async Task Sut_correctly_fails_with_duplicate_username(
        SqlUserReader reader,
        IPasswordHasher hasher,
        SqlUserRepository repository,
        CreateUser command)
    {
        CreateUserCommandExecutor sut = new(reader, hasher, repository);
        await sut.Execute(id: Guid.NewGuid(), command);
        
        Func<Task> action = () => sut.Execute(id: Guid.NewGuid(), command);

        await action.Should().ThrowAsync<InvariantViolationException>();
    }
}