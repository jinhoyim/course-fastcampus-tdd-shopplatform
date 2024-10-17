using Microsoft.EntityFrameworkCore;
using Orders.Domain;

namespace Orders.Infrastructure;

public class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options)
        : base(options)
    {
        
    }
    
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var orderBuilder = modelBuilder.Entity<Order>();
        orderBuilder.HasKey(x => x.Sequence);
        orderBuilder.Property(x => x.Sequence).ValueGeneratedOnAdd();
        orderBuilder.HasIndex(x => x.Id).IsUnique();
        orderBuilder.HasIndex(x => x.UserId);
        orderBuilder.HasIndex(x => x.ShopId);
        orderBuilder.Property(x => x.Status)
            .HasConversion<string>()
            .HasColumnType("varchar(20)");
        orderBuilder.HasIndex(x => x.Status);
    }
}