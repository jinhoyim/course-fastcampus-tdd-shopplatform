using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Orders.Application.Commands;
using Orders.Application.Events;
using Orders.Domain.Model;
using Sellers;
using Sellers.UnitTests;

namespace Orders.UnitTests;

public static class TestSpecifiedLanguage
{
    public static SellersServer GetSellersServer(this OrdersServer server)
    {
        return server.Services.GetRequiredService<SellersServer>();
    }
    
    public static async Task<HttpResponseMessage> PlaceOrder(
        this OrdersServer server,
        Guid orderId,
        Guid? shopId = null)
    {
        string uri = $"/api/v1/orders/{orderId}/place-order";

        if (shopId == null)
        {
            Shop shop = await server.GetSellersServer().CreateShop();
            shopId = shop.Id;
        }
        
        PlaceOrder body = new(
            UserId: Guid.NewGuid(),
            ShopId: shopId.Value,
            ItemId: Guid.NewGuid(),
            Price: 10000);
        
        return await server.CreateClient().PostAsJsonAsync(uri, body);
    }

    public static async Task<HttpResponseMessage> StartOrder(
        this OrdersServer server,
        Guid orderId,
        string? paymentTransactionId = null)
    {
        HttpClient client = server.CreateClient();
        string uri = $"api/v1/orders/{orderId}/start-order";
        StartOrder body = new(paymentTransactionId);
        return await client.PostAsJsonAsync(uri, body);
    }

    public static async Task<HttpResponseMessage> HandleBankTransferPaymentCompleted(this OrdersServer server, Guid orderId)
    {
        HttpClient client = server.CreateClient();
        string uri = "api/v1/orders/handle/bank-transfer-payment-completed";
        BankTransferPaymentCompleted body = new(orderId, EventTimeUtc: DateTime.UtcNow);
        return await client.PostAsJsonAsync(uri, body);
    }

    public static async Task<HttpResponseMessage> HandleItemShipped(this OrdersServer server, Guid orderId)
    {
        HttpClient client = server.CreateClient();
        string uri = "api/v1/orders/handle/item-shipped";
        ItemShipped body = new(orderId, EventTimeUtc: DateTime.UtcNow);
        return await client.PostAsJsonAsync(uri, body);
    }

    public static async Task<Order?> FindOrder(this OrdersServer server, Guid orderId)
    {
        HttpClient client = server.CreateClient();
        string uri = $"api/v1/orders/{orderId}";
        return await client.GetFromJsonAsync<Order>(uri);
    }
}