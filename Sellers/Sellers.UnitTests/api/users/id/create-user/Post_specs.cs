using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Sellers.Commands;

namespace Sellers.api.users.id.create_user;

public class Post_specs
{
    [Theory, AutoSellersData]
    public async Task Sut_correctly_creates_user(
        SellersServer server,
        Guid userId,
        CreateUser command)
    {
        string commandUri = $"api/users/{userId}/create-user";
        HttpClient client = server.CreateClient();
        await client.PostAsJsonAsync(commandUri, command);

        string queryUri = "api/users/verify-password";
        Credentials credentials = new Credentials(command.Username, command.Password);
        HttpResponseMessage actual = await client.PostAsJsonAsync(queryUri, credentials);
        actual.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory, AutoSellersData]
    public async Task Sut_returns_BadRequest_with_duplicate_username(
        SellersServer server,
        CreateUser command)
    {
        using HttpClient client = server.CreateClient();
        await client.PostAsJsonAsync($"api/users/{Guid.NewGuid()}/create-user", command);

        string uri = $"api/users/{Guid.NewGuid()}/create-user";
        HttpResponseMessage response = await client.PostAsJsonAsync(uri, command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}