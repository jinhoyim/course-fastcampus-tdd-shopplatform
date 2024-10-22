using Orders.Domain.Model;
using Orders.Domain.Model.Specifications;

namespace Orders.Domain;

public interface IOrderRepository
{
    IUnitOfWork UnitOfWork { get; }
    Task<IEnumerable<Order>> GetAllOrders(IEnumerable<ISpecification<Order>>? specifications = null, bool trackChanges = true);
    Task<Order?> FindOrderById(Guid orderId, bool trackChanges = true);
    Task Add(Order order);
    
}