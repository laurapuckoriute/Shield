using Shield.Api.Models;

namespace Shield.Api.Services;

public sealed class PaymentFraudAssessmentService : IPaymentFraudAssessmentService
{
    public PaymentFraudAssessmentResult Assess(PaymentFraudCheckRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var riskScore = 0.0;
        var reasons = new List<PaymentFraudReason>();

        if (request.Amount >= 5000)
        {
            riskScore += 0.4;
            reasons.Add(PaymentFraudReason.HighTransactionAmount);
        }

        if (!string.Equals(request.Payer.Country, request.Network.Country, StringComparison.OrdinalIgnoreCase))
        {
            riskScore += 0.15;
            reasons.Add(PaymentFraudReason.IpCountryMismatch);
        }

        if (request.Payee.IsNewPayee)
        {
            riskScore += 0.2;
            reasons.Add(PaymentFraudReason.NewPayee);
        }

        if (request.Device.IsNewDevice)
        {
            riskScore += 0.15;
            reasons.Add(PaymentFraudReason.NewDevice);
        }

        if (request.AuthContext.FailedLoginsLast24h > 0)
        {
            riskScore += 0.1;
            reasons.Add(PaymentFraudReason.RecentFailedLogins);
        }

        if (request.Payee.MerchantCategory == PaymentMerchantCategory.Crypto)
        {
            riskScore += 0.1;
            reasons.Add(PaymentFraudReason.HighRiskMerchantCategory);
        }

        riskScore = Math.Clamp(riskScore, 0, 1);

        return new PaymentFraudAssessmentResult
        {
            IsLikelyFraud = riskScore >= 0.5,
            RiskScore = Math.Round(riskScore, 2),
            Reasons = reasons
        };
    }
}
