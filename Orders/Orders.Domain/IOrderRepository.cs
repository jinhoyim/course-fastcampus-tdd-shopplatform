using Orders.Model;
using Orders.Model.Specifications;

namespace Orders;

public interface IOrderRepository
{
    IUnitOfWork UnitOfWork { get; }
    Task<IEnumerable<Order>> GetAllOrders(IEnumerable<ISpecification<Order>>? specifications = null, bool trackChanges = true);
    Task<Order?> FindOrderById(Guid orderId, bool trackChanges = true);
    Task Add(Order order);
    
}