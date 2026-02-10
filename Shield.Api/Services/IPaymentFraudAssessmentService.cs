using Shield.Api.Models;

namespace Shield.Api.Services;

public interface IPaymentFraudAssessmentService
{
    PaymentFraudAssessmentResult Assess(PaymentFraudCheckRequest request);
}
