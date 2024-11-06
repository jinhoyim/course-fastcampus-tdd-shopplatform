using System.Linq.Expressions;

namespace Orders.Model.Specifications;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
}