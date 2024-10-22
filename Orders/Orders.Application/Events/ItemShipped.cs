namespace Orders.Application.Events;

public sealed record ItemShipped(
    Guid OrderId,
    DateTime EventTimeUtc);