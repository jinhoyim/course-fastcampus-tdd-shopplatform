namespace Orders.Application.Events;

public sealed record BankTransferPaymentCompleted(
    Guid OrderId,
    DateTime EventTimeUtc);