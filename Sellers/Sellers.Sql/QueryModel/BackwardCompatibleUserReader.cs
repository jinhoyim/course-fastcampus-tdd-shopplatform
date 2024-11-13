namespace Sellers.QueryModel;

public class BackwardCompatibleUserReader : IUserReader
{
    private readonly IEnumerable<IUserReader> _readers;

    public BackwardCompatibleUserReader(Func<SellersDbContext> contextFactory)
    : this(
        new SqlUserReader(contextFactory),
        new ShopUserReader(contextFactory))
    {
    }

    public BackwardCompatibleUserReader(params IUserReader[] readers)
    {
        _readers = readers.AsEnumerable();
    }

    public async Task<User?> FindUser(string username)
    {
        return await FindUser(async reader => await reader.FindUser(username));
    }

    public async Task<User?> FindUser(Guid id)
    {
        return await FindUser(async reader => await reader.FindUser(id));
    }

    private async Task<User?> FindUser(Func<IUserReader, ValueTask<User?>> selector)
    {
        return await _readers.ToAsyncEnumerable()
            .SelectAwait(selector)
            .Where(user => user is not null)
            .FirstOrDefaultAsync();
    }
}