namespace Orders.Application.Events;

public sealed record PaymentApproved(
    string PaymentTransactionId,
    DateTime EventTimeUtc);