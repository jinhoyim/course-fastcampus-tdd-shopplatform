using System.Collections.Immutable;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Sellers.QueryModel;

public sealed class ShopUserReader : IUserReader
{
    private readonly Func<SellersDbContext> contextFactory;

    public ShopUserReader(Func<SellersDbContext> contextFactory)
    {
        this.contextFactory = contextFactory;
    }
    
    public Task<User?> FindUser(string username) => FindUser(x => x.UserId == username);

    public Task<User?> FindUser(Guid id) => FindUser(x => x.Id == id);

    private async Task<User?> FindUser(Expression<Func<Shop, bool>> predicate)
    {
        await using SellersDbContext dbContext = contextFactory();
        IQueryable<Shop> query = dbContext.Shops
            .AsNoTracking()
            .Where(predicate);

        return await query.SingleOrDefaultAsync() switch
        {
            {} shop => new User(
                shop.Id,
                shop.UserId!,
                shop.PasswordHash!,
                Roles: GetUserRoles(shop)),
            null => null,
        };
    }

    private static ImmutableArray<Role> GetUserRoles(Shop shop)
        => [new Role(shop.Id, RoleName: "Administrator")];
}