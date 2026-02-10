using Shield.Api.Models;

namespace Shield.Api.Services;

public interface IPaymentFraudAssessmentService
{
    Task<PaymentFraudAssessmentResult> Assess(
        PaymentFraudCheckRequest request,
        CancellationToken cancellationToken = default);
}
