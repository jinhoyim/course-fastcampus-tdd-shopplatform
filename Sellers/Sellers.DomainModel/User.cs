using System.Collections.Immutable;
using Sellers.Commands;

namespace Sellers;

public sealed record User(
    Guid Id,
    string Username,
    string PasswordHash,
    ImmutableArray<Role> Roles)
{
    internal User GrantRole(GrantRole command)
    {
        Role newRole = new Role(command.ShopId, command.RoleName);
        return this with { Roles = Roles.Add(newRole) };
    }
}