using System.Linq.Expressions;

namespace Orders.Domain.Model.Specifications;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
}