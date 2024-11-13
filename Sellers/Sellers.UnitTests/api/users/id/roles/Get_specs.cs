using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Sellers.api.users.id.roles;

public class Get_specs
{
    [Theory, AutoSellersData]
    public async Task Sut_returns_NotFound_with_nonexistent_id(
        SellersServer server,
        Guid id)
    {
        string uri = $"api/users/{id}/roles";
        HttpResponseMessage response = await server.CreateClient().GetAsync(uri);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory, AutoSellersData]
    public async Task Sut_returns_OK_with_existing_id(
        SellersServer server,
        Shop shop)
    {
        IServiceScope scope = server.Services.CreateScope();
        SellersDbContext dbContext = scope.ServiceProvider.GetRequiredService<SellersDbContext>();
        dbContext.Shops.Add(shop);
        await dbContext.SaveChangesAsync();

        string uri = $"api/users/{shop.Id}/roles";
        HttpResponseMessage response = await server.CreateClient().GetAsync(uri);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory, AutoSellersData]
    public async Task Sut_correctly_return_roles(
        SellersServer server,
        Shop shop)
    {
        IServiceScope scope = server.Services.CreateScope();
        SellersDbContext dbContext = scope.ServiceProvider.GetRequiredService<SellersDbContext>();
        dbContext.Shops.Add(shop);
        await dbContext.SaveChangesAsync();

        string uri = $"api/users/{shop.Id}/roles";
        HttpResponseMessage response = await server.CreateClient().GetAsync(uri);
        
        Role[] actual = await response.Content.ReadFromJsonAsync<Role[]>();
        Role role = new Role(shop.Id, RoleName: "Administrator");
        actual.Should().BeEquivalentTo([role]);
    }
}