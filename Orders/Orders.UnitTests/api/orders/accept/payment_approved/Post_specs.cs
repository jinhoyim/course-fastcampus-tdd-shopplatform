using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Orders.Events;
using Orders.Model;

namespace Orders.api.orders.accept.payment_approved;

public class Post_specs
{
    [Fact]
    public async Task Sut_returns_Accepted_status_code()
    {
        // Arrange
        HttpClient client = OrdersServer.Create().CreateClient();
        
        string uri = "api/v1/orders/accept/payment-approved";
        string tid = Guid.NewGuid().ToString();
        DateTime approvedAt = DateTime.UtcNow;
        ExternalPaymentApproved body = new ExternalPaymentApproved(tid, approvedAt);

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(uri, body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task Sut_correctly_changes_order_status()
    {
        // Arrange
        OrdersServer server = OrdersServer.Create();
        
        Guid orderId = Guid.NewGuid();
        await server.PlaceOrder(orderId);
        string paymentTransactionId = Guid.NewGuid().ToString();
        await server.StartOrder(orderId, paymentTransactionId);
        
        ExternalPaymentApproved paymentApproved = new(
            tid: paymentTransactionId,
            approved_at: DateTime.UtcNow);
        
        // Act
        string uri = "api/v1/orders/accept/payment-approved";
        await server.CreateClient().PostAsJsonAsync(uri, paymentApproved);
        
        // Assert
        await DefaultPolicy.Instance.ExecuteAsync(async () =>
        {
            Order? actual = await server.FindOrder(orderId);
            actual!.Status.Should().Be(OrderStatus.AwaitingShipment);
        });
    }

    [Fact]
    public async Task Sut_correctly_sets_event_time()
    {
        // Arrange
        OrdersServer server = OrdersServer.Create();
        
        Guid orderId = Guid.NewGuid();
        await server.PlaceOrder(orderId);
        string paymentTransactionId = Guid.NewGuid().ToString();
        await server.StartOrder(orderId, paymentTransactionId);
        
        ExternalPaymentApproved paymentApproved = new(
            tid: paymentTransactionId,
            approved_at: DateTime.UtcNow);
        
        // Act
        string uri = "api/v1/orders/accept/payment-approved";
        await server.CreateClient().PostAsJsonAsync(uri, paymentApproved);
        
        // Assert
        await DefaultPolicy.Instance.ExecuteAsync(async () =>
        {
            Order? actual = await server.FindOrder(orderId);
            actual!.PaidAtUtc.Should().BeCloseTo(paymentApproved.approved_at, precision: TimeSpan.FromMilliseconds(1));
        });
    }
}