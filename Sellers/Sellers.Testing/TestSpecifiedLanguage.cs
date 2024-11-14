using System.Net.Http.Json;
using Sellers.Commands;

namespace Sellers;

public static class TestSpecifiedLanguage
{
    public static async Task<Shop> CreateShop(this SellersServer server)
    {
        using HttpClient client = server.CreateClient();
        string uri = "api/shops";
        var body = new { Name = $"{Guid.NewGuid()}" };
        HttpResponseMessage response = await client.PostAsJsonAsync(uri, body);
        return (await response.Content.ReadFromJsonAsync<Shop>())!;
    }

    public static async Task<ShopView> CreateShop(
        this SellersServer server,
        string name)
    {
        using HttpClient client = server.CreateClient();
        string uri = "api/shops";
        var body = new { name };
        HttpResponseMessage response = await client.PostAsJsonAsync(uri, body);
        return (await response.Content.ReadFromJsonAsync<ShopView>())!;
    }
    
    public static async Task SetShopUser(
        this SellersServer server,
        Guid shopId,
        string userId,
        string password)
    {
        string uri = $"api/shops/{shopId}/user";
        ShopUser body = new ShopUser(userId, password);
        await server.CreateClient().PostAsJsonAsync(uri, body);
    }

    public static async Task<ShopView> GetShop(
        this SellersServer server,
        Guid shopId)
    {
        using HttpClient client = server.CreateClient();
        string uri = $"api/shops/{shopId}";
        HttpResponseMessage response = await client.GetAsync(uri);
        HttpContent content = response.EnsureSuccessStatusCode().Content;
        return (await content.ReadFromJsonAsync<ShopView>())!;
    }

    public static async Task CreateUser(
        this SellersServer server,
        Guid id,
        string username,
        string password)
    {
        HttpClient client = server.CreateClient();
        CreateUser body = new(username, password);
        string uri = $"api/users/{id}/create-user";
        await client.PostAsJsonAsync(uri, body);
    }

    public static async Task<HttpResponseMessage> GrantRole(
        this SellersServer server,
        Guid id,
        Guid shopId,
        string roleName)
    {
        HttpClient client = server.CreateClient();
        string uri = $"api/users/{id}/grant-role";
        GrantRole body = new(shopId, roleName);
        return await client.PostAsJsonAsync(uri, body);
    }
    
    public static async Task<IEnumerable<Role>> GetRoles(
        this SellersServer server,
        Guid id)
    {
        HttpClient client = server.CreateClient();
        string uri = $"api/users/{id}/roles";
        HttpResponseMessage response = await client.GetAsync(uri);
        return (await response.Content.ReadFromJsonAsync<IEnumerable<Role>>())!;
    }
}