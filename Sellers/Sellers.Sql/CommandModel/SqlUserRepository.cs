using Microsoft.EntityFrameworkCore;

namespace Sellers.CommandModel;

public sealed class SqlUserRepository : IUserRepository
{
    private readonly Func<SellersDbContext> _contextFactory;

    public SqlUserRepository(Func<SellersDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task Add(User user)
    {
        await using SellersDbContext dbContext = _contextFactory();
        dbContext.Users.Add(new UserEntity
        {
            Id = user.Id,
            Username = user.Username,
            PasswordHash = user.PasswordHash
        });
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> TryUpdate(Guid id, Func<User, User> reviser)
    {
        await using SellersDbContext dbContext = _contextFactory();
        IQueryable<UserEntity> query = dbContext.Users
            .Include(x => x.Roles)
            .Where(x => x.Id == id);
        if (await query.SingleOrDefaultAsync() is { } entity)
        {
            User user = Mapper.Instance.Map<User>(entity);
            
            Mapper.Instance.Map(
                source: reviser(user),
                destination: entity,
                sourceType: typeof(User),
                destinationType: typeof(UserEntity));
            
            await dbContext.SaveChangesAsync();
            
            return true;
        }
        else
        {
            return false;
        }
    }
}