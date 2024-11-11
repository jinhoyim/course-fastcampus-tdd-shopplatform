using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Sellers.api.users.verify_password;

public class Post_specs
{
    [Theory, AutoSellersData]
    public async Task Sut_returns_BadRequest_with_invalid_credentials(
        SellersServer server,
        Credentials credentials)
    {
        HttpClient client = server.CreateClient();
        string uri = "api/users/verify-password";

        HttpResponseMessage response = await client.PostAsJsonAsync(uri, credentials);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory, AutoSellersData]
    public async Task Sut_returns_Ok_with_valid_credentials(
        SellersServer server,
        string shopName,
        string username,
        string password)
    {
        ShopView shop = await server.CreateShop(shopName);
        await server.SetShopUser(shop.Id, username, password);
        string uri = "api/users/verify-password";
        Credentials credentials = new(username, password);

        HttpClient client = server.CreateClient();
        HttpResponseMessage response = await client.PostAsJsonAsync(uri, credentials);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}