using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sellers;

public sealed class Shop
{
    public Guid Id { get; set; }

    [JsonIgnore]
    public int Sequence { get; set; }

    [Required]
    public string Name { get; set; }
    
    public string? UserId { get; set; }

    public string? PasswordHash { get; set; }
}