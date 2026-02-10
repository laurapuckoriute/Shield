using Shield.Api.Models;

namespace Shield.Api.Messaging.Events;

public sealed record PayeeVerificationRequested
{
    public required string TransactionId { get; init; }
    public required string UserId { get; init; }
    public required decimal Amount { get; init; }
    public required string Currency { get; init; }
    public required DateTime Timestamp { get; init; }
    public required string PayeeAccountId { get; init; }
    public required PaymentMerchantCategory MerchantCategory { get; init; }
    public required PaymentFraudReason Reason { get; init; }
}
