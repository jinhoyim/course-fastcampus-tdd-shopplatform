using Orders.Domain.Model;

namespace Orders.Api;

public static class QueryExtensions
{
    public static IQueryable<Order> FilterByUser(this IQueryable<Order> source, Guid? userId)
        => userId == null ? source : source.Where(x => x.UserId == userId);

    public static IQueryable<Order> FilterByShop(this IQueryable<Order> source, Guid? shopId)
        => shopId == null ? source : source.Where(x => x.ShopId == shopId);
}