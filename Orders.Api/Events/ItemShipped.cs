namespace Orders.Api.Events;

public sealed record ItemShipped(
    Guid OrderID,
    DateTime EventTimeUtc);