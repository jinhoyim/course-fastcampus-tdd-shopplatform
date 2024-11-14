using Sellers.Commands;

namespace Sellers.CommandModel;

public sealed class RevokeRoleCommandExecutor
{
    private readonly IUserRepository _repository;

    public RevokeRoleCommandExecutor(IUserRepository repository)
        => _repository = repository;

    public async Task Execute(Guid id, RevokeRole command)
    {
        if (await _repository.TryUpdate(id, x => x.RevokeRole(command)) == false)
        {
            throw new EntityNotFoundException();
        }
    }
}