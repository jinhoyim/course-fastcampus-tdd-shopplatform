using System.Net.Http.Json;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Sellers.api.shops.id;

public class Get_specs
{
    [Fact]
    public async Task Sut_does_not_expose_user_credentials()
    {
        // Arrange
        SellersServer server = SellersServer.Create();

        using IServiceScope scope = server.Services.CreateScope();
        SellersDbContext context = scope.ServiceProvider.GetRequiredService<SellersDbContext>();
        Shop shop = new()
        {
            Id = Guid.NewGuid(),
            Name = $"{Guid.NewGuid()}",
            UserId = $"{Guid.NewGuid()}",
            PasswordHash = $"{Guid.NewGuid()}"
        };
        context.Shops.Add(shop);
        await context.SaveChangesAsync();
        
        // Act
        HttpClient client = server.CreateClient();
        HttpResponseMessage response = await client.GetAsync($"api/shops/{shop.Id}");
        Shop? actual = await response.Content.ReadFromJsonAsync<Shop>();
        
        // Assert
        actual.UserId.Should().BeNull();
        actual.PasswordHash.Should().BeNull();
    }
    
    [Fact]
    public async Task Sut_does_not_expose_user_information()
    {
        // Arrange
        SellersServer server = SellersServer.Create();
        var shop = await server.CreateShop();
        await server.SetShopUser(shop.Id, $"{Guid.NewGuid()}", $"password 1");
        
        // Act
        string uri = $"api/shops/{shop.Id}";
        HttpResponseMessage response = await server.CreateClient().GetAsync(uri);
        Shop? actual = await response.Content.ReadFromJsonAsync<Shop>();

        // Assert
        actual!.UserId.Should().BeNull();
        actual.PasswordHash.Should().BeNull();
    }
}