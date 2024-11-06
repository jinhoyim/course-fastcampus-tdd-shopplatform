using Microsoft.EntityFrameworkCore;
using Orders.Model;
using Orders.Model.Specifications;

namespace Orders;

public sealed class OrderRepository : IOrderRepository
{
    private readonly OrdersDbContext _dbContext;

    public OrderRepository(OrdersDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public IUnitOfWork UnitOfWork => _dbContext;

    public async Task<IEnumerable<Order>> GetAllOrders(IEnumerable<ISpecification<Order>>? specifications = null,
        bool trackChanges = true)
    {
        var query = trackChanges ? _dbContext.Orders : _dbContext.Orders.AsQueryable();

        if (specifications != null)
        {
            query = specifications.Aggregate(query, (current, specification) => current.Where(specification.Criteria));
        }

        return await query.ToListAsync();
    }

    public async Task<Order?> FindOrderById(Guid orderId, bool trackChanges = false)
    {
        var query = trackChanges ? _dbContext.Orders : _dbContext.Orders.AsQueryable();
        return await query.FirstOrDefaultAsync(x => x.Id == orderId);
    }

    public async Task Add(Order order)
    {
        await _dbContext.Orders.AddAsync(order);
    }

}