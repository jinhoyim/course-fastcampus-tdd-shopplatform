using System.Net.Mime;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Sellers.Api.Controllers;

[ApiController]
[Route("api/shops")]
public sealed class ShopsController : ControllerBase
{
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json, Type = typeof(Shop))]
    public async Task<IActionResult> CreateShop(
        [FromBody] Shop shop,
        [FromServices] SellersDbContext context)
    {
        shop.Id = Guid.NewGuid();
        context.Shops.Add(shop);
        await context.SaveChangesAsync();
        return Ok(shop);
    }
    
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json, Type = typeof(Shop[]))]
    public async Task<IEnumerable<Shop>> GetShops([FromServices] SellersDbContext context)
    {
        return await context.Shops.AsNoTracking().ToListAsync();
    }

    [HttpGet("{id}")]
    [Produces(MediaTypeNames.Application.Json, Type = typeof(Shop))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FindShop(
        Guid id,
        [FromServices] SellersDbContext context)
    {
        return await context.Shops.FindShop(id) switch
        {
            { } shop => Ok(shop),
            null => NotFound(),
        };
    }

    [HttpPost("{id}/user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PostUser(
        Guid id,
        [FromBody] ShopUser user,
        [FromServices] SellersDbContext context,
        [FromServices] IPasswordHasher<object> hasher)
    {
        Shop? shop = await context.Shops.FindShop(id);
        if (shop == null)
        {
            return NotFound();
        }

        shop.UserId = user.Id;
        shop.PasswordHash = hasher.HashPassword(user, user.Password);

        await context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("verify-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyPassword(
        [FromBody] ShopUser user,
        [FromServices] SellersDbContext context,
        [FromServices] IPasswordHasher<object> hasher)
    {
        IQueryable<Shop> query =
            from s in context.Shops
            where s.UserId == user.Id
            select s;

        Shop? shop = await query.SingleOrDefaultAsync();

        if (shop == null || shop.PasswordHash == null)
        {
            return BadRequest();
        }
        else
        {
            PasswordVerificationResult result = hasher.VerifyHashedPassword(
                user,
                shop.PasswordHash,
                user.Password);

            return result == PasswordVerificationResult.Failed
                ? BadRequest()
                : Ok();
        }
    }
}