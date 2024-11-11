using Microsoft.EntityFrameworkCore;

namespace Sellers;

public static class SellersDatabase
{
    public const string DefaultConnectionString = "Host=localhost;port=5432;Database=SellersDB_Testing;Username=testuser;Password=mysecret-pp#";

    public static SellersDbContext GetContext(string connectionString = DefaultConnectionString)
    {
        DbContextOptionsBuilder<SellersDbContext> builder = new();
        return new(builder.UseNpgsql(connectionString).Options);
    }
}