using Sellers.Commands;
using Sellers.QueryModel;

namespace Sellers.CommandModel;

public sealed class CreateUserCommandExecutor
{
    private readonly IUserReader _reader;
    private readonly IPasswordHasher _hasher;
    private readonly IUserRepository _repository;

    public CreateUserCommandExecutor(
        IUserReader reader,
        IPasswordHasher hasher,
        IUserRepository repository)
    {
        _reader = reader;
        _hasher = hasher;
        _repository = repository;
    }

    public async Task Execute(Guid id, CreateUser command)
    {
        await AssertThatUsernameIsUnique(command.Username);
        await AddUser(id, command);
    }

    private async Task AddUser(Guid id, CreateUser command)
    {
        User user = new User(id, command.Username, _hasher.HashPassword(command.Password));
        await _repository.Add(user);
    }

    private async Task AssertThatUsernameIsUnique(string username)
    {
        if (await _reader.FindUser(username) is not null)
        {
            throw new InvariantViolationException();
        }
    }
}