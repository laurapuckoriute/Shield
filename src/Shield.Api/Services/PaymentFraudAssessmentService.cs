using Shield.Api.Messaging.Events;
using Shield.Api.Models;
using Wolverine;

namespace Shield.Api.Services;

public sealed class PaymentFraudAssessmentService(
    IMessageContext messageContext,
    ILogger<PaymentFraudAssessmentService> logger)
    : IPaymentFraudAssessmentService
{
    public async Task<PaymentFraudAssessmentResult> Assess(PaymentFraudCheckRequest request, CancellationToken cancellationToken = default)
    {
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
        var roundedRiskScore = Math.Round(riskScore, 2);

        var assessment = new PaymentFraudAssessmentResult
        {
            IsLikelyFraud = roundedRiskScore >= 0.5,
            RiskScore = roundedRiskScore,
            Reasons = reasons
        };

        await PublishMessagesAsync(request, reasons);

        return assessment;
    }

    private async Task PublishMessagesAsync(
        PaymentFraudCheckRequest request,
        IReadOnlyCollection<PaymentFraudReason> reasons)
    {
        if (reasons.Contains(PaymentFraudReason.RecentFailedLogins))
        {
            await messageContext.PublishAsync(
                new HumanInterventionRequested
                {
                    TransactionId = request.TransactionId,
                    UserId = request.UserId,
                    Amount = request.Amount,
                    Currency = request.Currency,
                    Timestamp = request.Timestamp,
                    FailedLoginsLast24h = request.AuthContext.FailedLoginsLast24h,
                    Reason = PaymentFraudReason.RecentFailedLogins
                });

            logger.LogDebug(
                "Published HumanInterventionRequested for transaction {TransactionId}.",
                request.TransactionId);
        }

        if (reasons.Contains(PaymentFraudReason.NewPayee))
        {
            await messageContext.PublishAsync(
                new PayeeVerificationRequested
                {
                    TransactionId = request.TransactionId,
                    UserId = request.UserId,
                    Amount = request.Amount,
                    Currency = request.Currency,
                    Timestamp = request.Timestamp,
                    PayeeAccountId = request.Payee.AccountId,
                    MerchantCategory = request.Payee.MerchantCategory,
                    Reason = PaymentFraudReason.NewPayee
                });

            logger.LogDebug(
                "Published PayeeVerificationRequested for transaction {TransactionId}.",
                request.TransactionId);
        }
    }
}
