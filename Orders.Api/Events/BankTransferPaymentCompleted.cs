namespace Orders.Api.Events;

public sealed record BankTransferPaymentCompleted(
    Guid OrderId,
    DateTime EventTimeUtc);