using System.Text.Json.Serialization;

namespace Orders.Domain.Model;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderStatus
{
    Pending,
    AwaitingPayment,
    AwaitingShipment,
    Completed,
}