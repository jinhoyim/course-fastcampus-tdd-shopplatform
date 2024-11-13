namespace Sellers.QueryModel;

public interface IUserReader
{
    Task<User?> FindUser(string username);
    Task<User?> FindUser(Guid id);
}