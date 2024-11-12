using Microsoft.EntityFrameworkCore;

namespace Sellers.QueryModel;

public sealed class SqlUserReader : IUserReader
{
    private readonly Func<SellersDbContext> _contextFactory;

    public SqlUserReader(Func<SellersDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
    
    public async Task<User?> FindUser(string username)
    {
        await using SellersDbContext dbContext = _contextFactory();
        IQueryable<UserEntity> query = dbContext.Users
            .AsNoTracking()
            .Where(x => x.Username == username);
        return await query.SingleOrDefaultAsync() switch
        {
            { } user => new(
                user.Id,
                user.Username,
                user.PasswordHash),
            null => null
        };
    }
}