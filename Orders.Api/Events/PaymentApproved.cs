namespace Orders.Api.Events;

public sealed record PaymentApproved(
    string PaymentTransactionId,
    DateTime EventTimeUtc);