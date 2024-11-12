using AutoFixture;
using Microsoft.AspNetCore.Identity;
using Sellers.QueryModel;

namespace Sellers;

public class PasswordHasherCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Register<IPasswordHasher<object>>(() => new PasswordHasher<object>());
        fixture.Register(GetPasswordHasher);
    }

    private static IPasswordHasher GetPasswordHasher()
        => new AspNetCorePasswordHasher(new PasswordHasher<object>());
}