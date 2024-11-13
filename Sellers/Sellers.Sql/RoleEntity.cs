namespace Sellers;

public sealed class RoleEntity
{
    public long UserSequence { get; init; }

    public Guid ShopId { get; init; }
    
    public string RoleName { get; init; }
    
    public UserEntity User { get; init; }
}