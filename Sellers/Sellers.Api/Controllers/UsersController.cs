using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Sellers.CommandModel;
using Sellers.Commands;
using Sellers.Filters;
using Sellers.QueryModel;

namespace Sellers.Controllers;

[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    [HttpPost("verify-password")]
    public async Task<IActionResult> VerifyPassword(
        [FromBody] Credentials credentials,
        [FromServices] PasswordVerifier verifier)
    {
        return await verifier.VerifyPassword(credentials.Username, credentials.Password) switch
        {
            true => Ok(),
            false => BadRequest()
        };
    }

    [HttpPost("{id}/create-user")]
    [TypeFilter<InvariantViolationFilter>]
    public Task CreateUser(
        Guid id,
        [FromBody] CreateUser command,
        [FromServices] CreateUserCommandExecutor executor)
    {
        return executor.Execute(id, command);
    }

    [HttpGet("{id}/roles")]
    public async Task<IActionResult> GetRoles(Guid id, [FromServices] IUserReader userReader)
    {
        return await userReader.FindUser(id) is { } user
            ? Ok(user.Roles)
            : NotFound();
    }

    [HttpPost("{id}/grant-role")]
    [TypeFilter(typeof(EntityNotFoundFilter))]
    public Task GrantRole(
        Guid id,
        [FromBody] GrantRole command,
        [FromServices] GrantRoleCommandExecutor executor)
    {
        return executor.Execute(id, command);
    }

    [HttpPost("{id}/revoke-role")]
    [TypeFilter(typeof(EntityNotFoundFilter))]
    public Task RevokeRole(
        Guid id,
        [FromBody] RevokeRole command,
        [FromServices] RevokeRoleCommandExecutor executor)
    {
        return executor.Execute(id, command);
    }
}
