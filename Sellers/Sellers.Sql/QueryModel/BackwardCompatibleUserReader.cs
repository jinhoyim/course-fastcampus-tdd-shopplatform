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
        foreach (IUserReader reader in _readers)
        {
            if (await reader.FindUser(username) is { } user)
            {
                return user;
            }
        }
        return default;
    }
}