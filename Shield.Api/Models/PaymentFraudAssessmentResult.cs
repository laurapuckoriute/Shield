namespace Shield.Api.Models;

public sealed class PaymentFraudAssessmentResult
{
    public bool IsLikelyFraud { get; init; }
    public double RiskScore { get; init; }
    public IReadOnlyCollection<PaymentFraudReason> Reasons { get; init; } = Array.Empty<PaymentFraudReason>();
}
