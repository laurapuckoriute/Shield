using Shield.Api.Models;

namespace Shield.Api.Messaging.Events;

public sealed record HumanInterventionRequested
{
    public required string TransactionId { get; init; }
    public required string UserId { get; init; }
    public required decimal Amount { get; init; }
    public required string Currency { get; init; }
    public required DateTime Timestamp { get; init; }
    public required int FailedLoginsLast24h { get; init; }
    public required PaymentFraudReason Reason { get; init; }
}
