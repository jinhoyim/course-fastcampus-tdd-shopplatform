using System.Text.Json.Serialization;

namespace Orders.Domain;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderStatus
{
    Pending,
    AwaitingPayment,
    AwaitingShipment,
    Completed,
}