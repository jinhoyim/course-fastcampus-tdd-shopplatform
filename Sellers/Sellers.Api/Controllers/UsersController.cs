using Microsoft.AspNetCore.Mvc;
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
}
