using System.Net.Http.Json;
using Orders.Api.Commands;
using Orders.Api.Events;
using Orders.Domain.Model;

namespace Orders.UnitTests;

public static class TestSpecifiedLanguage
{
    public static async Task<Guid> PlaceOrder(this OrdersServer server)
    {
        string uri = "/api/v1/orders";
        
        PlaceOrder body = new(
            UserId: Guid.NewGuid(),
            ShopId: Guid.NewGuid(),
            ItemId: Guid.NewGuid(),
            Price: 10000);
        
        var response = await server.CreateClient().PostAsJsonAsync(uri, body);
        return (await response.Content.ReadFromJsonAsync<Order>())!.Id;
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