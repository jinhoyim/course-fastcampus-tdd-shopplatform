using System.Collections.Immutable;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Sellers.QueryModel;

public sealed class SqlUserReader : IUserReader
{
    private readonly Func<SellersDbContext> _contextFactory;

    public SqlUserReader(Func<SellersDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
    
    public Task<User?> FindUser(string username) => FindUser(x => x.Username == username);

    public Task<User?> FindUser(Guid id) => FindUser(x => x.Id == id);

    private async Task<User?> FindUser(Expression<Func<UserEntity, bool>> predicate)
    {
        await using SellersDbContext dbContext = _contextFactory();
        IQueryable<UserEntity> query = dbContext.Users
            .AsNoTracking()
            .Where(predicate);
        
        return await query.SingleOrDefaultAsync() switch
        {
            { } user => new(
                user.Id,
                user.Username,
                user.PasswordHash,
                ImmutableArray<Role>.Empty),
            null => null
        };
    }
}