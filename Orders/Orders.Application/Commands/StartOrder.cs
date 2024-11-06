namespace Orders.Commands;


// paymentTransactionId는 계좌이체 시나리오가 아닌 결제 서비스 시나리오에만 사용된다.
public sealed record StartOrder(string? PaymentTransactionId = null);