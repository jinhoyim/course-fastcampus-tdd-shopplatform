using AutoFixture;
using Microsoft.AspNetCore.Identity;

namespace Sellers;

public class PasswordHasherCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Register<IPasswordHasher<object>>(() => new PasswordHasher<object>());
    }
}