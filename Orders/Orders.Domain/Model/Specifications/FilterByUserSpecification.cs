using System.Linq.Expressions;

namespace Orders.Model.Specifications;

public class FilterByUserSpecification : ISpecification<Order>
{
    private readonly Guid? _userId;

    private FilterByUserSpecification(Guid? userId)
    {
        _userId = userId;
    }

    public Expression<Func<Order, bool>> Criteria =>
        _userId switch
        {
            null => _ => true,
            _ => order => order.UserId == _userId,
        };

    public static ISpecification<Order> Create(Guid? userId)
    {
        return new FilterByUserSpecification(userId);
    }
}