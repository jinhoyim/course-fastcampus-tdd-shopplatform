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
}
