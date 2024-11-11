using Microsoft.EntityFrameworkCore;

namespace Sellers.QueryModel;

public sealed class ShopUserReader : IUserReader
{
    private readonly Func<SellersDbContext> contextFactory;

    public ShopUserReader(Func<SellersDbContext> contextFactory)
    {
        this.contextFactory = contextFactory;
    }
    
    public async Task<User?> FindUser(string username)
    {
        SellersDbContext dbContext = contextFactory();
        IQueryable<Shop> query = dbContext.Shops
            .Where(x => x.UserId == username);
        
        return await query.SingleOrDefaultAsync() switch
        {
            { } shop => Translate(shop),
            null => null,
        };
    }

    private static User Translate(Shop shop)
    {
        return new User(shop.Id, shop.UserId!, shop.PasswordHash!);
    }
}