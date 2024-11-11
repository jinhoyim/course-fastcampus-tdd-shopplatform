using Microsoft.AspNetCore.Identity;

namespace Sellers.QueryModel;

public sealed class AspNetCorePasswordHasher : IPasswordHasher
{
    private static readonly object User = new();
    
    private readonly IPasswordHasher<object> _hasher;
    
    public AspNetCorePasswordHasher(IPasswordHasher<object> hasher) => this._hasher = hasher;

    public string HashPassword(string password)
    {
        return _hasher.HashPassword(User, password);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        return _hasher.VerifyHashedPassword(User, hashedPassword, providedPassword) switch
        {
            PasswordVerificationResult.Failed => false,
            _ => true
        };
    }
}