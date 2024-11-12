namespace Sellers.CommandModel;

public sealed class SqlUserRepository : IUserRepository
{
    private readonly Func<SellersDbContext> _contextFactory;

    public SqlUserRepository(Func<SellersDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task Add(User user)
    {
        await using SellersDbContext dbContext = _contextFactory();
        dbContext.Users.Add(new UserEntity
        {
            Id = user.Id,
            Username = user.Username,
            PasswordHash = user.PasswordHash
        });
        await dbContext.SaveChangesAsync();
    }
}