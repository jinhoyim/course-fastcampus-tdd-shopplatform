using System.Collections.Immutable;
using AutoFixture;

namespace Sellers;

public sealed class RoleCustomization : ICustomization
{
    public void Customize(IFixture fixture)
        => fixture.Register(() => fixture.CreateMany<Role>().ToImmutableArray());
}