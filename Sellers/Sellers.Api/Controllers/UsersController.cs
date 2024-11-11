using Microsoft.AspNetCore.Mvc;
using Sellers.QueryModel;

namespace Sellers.Controllers;

[Route("api/users/verify-password")]
public sealed class UsersController : ControllerBase
{
    [HttpPost]
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