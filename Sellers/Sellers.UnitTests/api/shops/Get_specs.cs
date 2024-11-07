using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Sellers.api.shops;

public class Get_specs
{
    private const string ConnectionString =
        "Server=127.0.0.1;port=5432;Database=SellersDB_Isolated;Username=testuser;Password=mysecret-pp#";
    
    [Theory, AutoSellersData]
    public async Task Sut_returns_all_shops([ConnectionString(ConnectionString)] SellersServer server, Shop[] shops)
    {
        using IServiceScope scope = server.Services.CreateScope();
        SellersDbContext context = scope.ServiceProvider.GetRequiredService<SellersDbContext>();
        context.Shops.RemoveRange(await context.Shops.ToListAsync());
        await context.Shops.AddRangeAsync(shops);
        await context.SaveChangesAsync();

        HttpClient client = server.CreateClient();
        HttpResponseMessage response = await client.GetAsync("api/shops");
        Shop[]? actual = await response.Content.ReadFromJsonAsync<Shop[]>();

        actual.Should().BeEquivalentTo(shops, s => s.Excluding(x => x.Sequence));
    }
}