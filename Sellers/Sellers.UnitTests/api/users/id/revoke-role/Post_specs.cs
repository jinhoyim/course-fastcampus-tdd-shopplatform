using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Sellers.Commands;

namespace Sellers.api.users.id.revoke_role;

public class Post_specs
{
    [Theory, AutoSellersData]
    public async Task Sut_correctly_removes_role(
        SellersServer server,
        Guid userId,
        string username,
        string password,
        Guid shopId,
        string roleName)
    {
        await server.CreateUser(userId, username, password);
        await server.GrantRole(userId, shopId, roleName);

        await server.RevokeRole(userId, shopId, roleName);

        IEnumerable<Role> actual = await server.GetRoles(userId);
        actual.Should().BeEmpty();
    }

    [Theory, AutoSellersData]
    public async Task Sut_returns_NotFound_status_code_if_user_not_exists(
        SellersServer server,
        Guid userId,
        Guid shopId,
        string roleName)
    {
        HttpResponseMessage response = await server.RevokeRole(userId, shopId, roleName);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}