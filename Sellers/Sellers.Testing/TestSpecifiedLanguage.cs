using System.Net.Http.Json;
using Sellers.Api;
using Sellers.Api.Controllers;

namespace Sellers.UnitTests;

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
}