using System.Net;
using FluentAssertions;

namespace Sellers.api.users.id.grant_role;

public class Post_specs
{
    [Theory, AutoSellersData]
    public async Task Sut_correctly_adds_role(
        SellersServer server,
        Guid id,
        string username,
        string password,
        Guid shopId,
        string roleName)
    {
        await server.CreateUser(id, username, password);

        await server.GrantRole(id, shopId, roleName);

        IEnumerable<Role> actual = await server.GetRoles(id);
        actual.Should().Contain(new Role(shopId, roleName));
    }

    [Theory, AutoSellersData]
    public async Task Sut_returns_NotFound_status_code_if_user_not_exists(
        SellersServer server,
        Guid userId,
        Guid shopId,
        string roleName)
    {
        HttpResponseMessage response = await server.GrantRole(userId, shopId, roleName);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}