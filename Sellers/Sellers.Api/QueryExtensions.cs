using Microsoft.EntityFrameworkCore;

namespace Sellers.Api;

public static class QueryExtensions
{
    public static Task<Shop?> FindShop(this DbSet<Shop> source, Guid id)
        => source.SingleOrDefaultAsync(x => x.Id == id);
}