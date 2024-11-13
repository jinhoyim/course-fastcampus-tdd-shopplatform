using System.Collections.Immutable;
using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Sellers.QueryModel;

public sealed class SqlUserReader : IUserReader
{
    private static readonly IMapper mapper = new MapperConfiguration(c =>
    {
        c.CreateMap<RoleEntity, Role>();
        c.CreateMap<UserEntity, User>();
        c.CreateMap<List<RoleEntity>, ImmutableArray<Role>>()
            .ConvertUsing(x => ConvertRoles(x));
    }).CreateMapper();

    private static ImmutableArray<Role> ConvertRoles(List<RoleEntity> source)
    {
        return [..source.Select(r => mapper.Map<Role>(r))];
    }
    
    private readonly Func<SellersDbContext> _contextFactory;

    public SqlUserReader(Func<SellersDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
    
    public Task<User?> FindUser(string username) => FindUser(x => x.Username == username);

    public Task<User?> FindUser(Guid id) => FindUser(x => x.Id == id);

    private async Task<User?> FindUser(Expression<Func<UserEntity, bool>> predicate)
    {
        await using SellersDbContext dbContext = _contextFactory();
        IQueryable<UserEntity> query = dbContext.Users
            .Include(u => u.Roles)
            .AsNoTracking()
            .Where(predicate);
        
        return await query.SingleOrDefaultAsync() switch
        {
            { } user => mapper.Map<User>(user),
            null => null
        };
    }
}