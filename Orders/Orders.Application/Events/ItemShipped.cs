namespace Orders.Application.Events;

public sealed record ItemShipped(
    Guid OrderID,
    DateTime EventTimeUtc);