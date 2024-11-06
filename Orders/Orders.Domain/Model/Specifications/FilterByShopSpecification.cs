using System.Linq.Expressions;

namespace Orders.Model.Specifications;

public sealed class FilterByShopSpecification : ISpecification<Order>
{
    private readonly Guid? _shopId;

    private FilterByShopSpecification(Guid? shopId)
    {
        _shopId = shopId;
    }

    // public Expression<Func<Order, bool>> Criteria => x => x.UserId == _userId;
    public Expression<Func<Order, bool>> Criteria =>
        _shopId switch
        {
            null => _ => true,
            _ => order => order.ShopId == _shopId
        };

    public static ISpecification<Order> Create(Guid? userId)
    {
        return new FilterByShopSpecification(userId);
    }
}