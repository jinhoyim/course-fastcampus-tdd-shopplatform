using Microsoft.EntityFrameworkCore;

namespace Sellers;

public static class SellersDatabase
{
    public const string DefaultConnectionString = "Host=localhost;port=5432;Database=SellersDB_Testing;Username=testuser;Password=mysecret-pp#";

    public static SellersDbContext CreateContext() => GetContext();
    
    public static SellersDbContext GetContext(string connectionString = DefaultConnectionString)
    {
        var dbContextOptions = GetDbContextOptions(connectionString);
        SellersDbContext dbContext = new SellersDbContext(dbContextOptions);
        dbContext.Database.Migrate();
        return dbContext;
    }

    private static DbContextOptions<SellersDbContext> GetDbContextOptions(string connectionString)
    {
        DbContextOptionsBuilder<SellersDbContext> builder = new();
        return builder.UseNpgsql(connectionString).Options;
    }
}