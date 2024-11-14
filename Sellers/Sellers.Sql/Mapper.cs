using System.Collections.Immutable;
using AutoMapper;

namespace Sellers;

internal static class Mapper
{
    public static IMapper Instance { get; } = new MapperConfiguration(c =>
    {
        c.CreateMap<Role, RoleEntity>();
        c.CreateMap<User, UserEntity>();
        c.CreateMap<RoleEntity, Role>();
        c.CreateMap<UserEntity, User>();
        c.CreateMap<List<RoleEntity>, ImmutableArray<Role>>()
            .ConvertUsing(x => ConvertRoles(x));
    }).CreateMapper();

    private static ImmutableArray<Role> ConvertRoles(List<RoleEntity> source)
    {
        return [..source.Select(r => Instance.Map<Role>(r))];
    }
}