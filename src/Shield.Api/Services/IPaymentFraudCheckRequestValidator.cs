using Shield.Api.Models;

namespace Shield.Api.Services;

public interface IPaymentFraudCheckRequestValidator
{
    ValidationResult Validate(PaymentFraudCheckRequest? request);
}
