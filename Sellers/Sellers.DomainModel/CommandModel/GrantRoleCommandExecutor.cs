using Sellers.Commands;

namespace Sellers.CommandModel;

public sealed class GrantRoleCommandExecutor
{
    private readonly IUserRepository _repository;

    public GrantRoleCommandExecutor(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task Execute(Guid id, GrantRole command)
    {
        if (await _repository.TryUpdate(id, x => x.GrantRole(command)) == false)
        {
            throw new EntityNotFoundException();
        }
    }
}