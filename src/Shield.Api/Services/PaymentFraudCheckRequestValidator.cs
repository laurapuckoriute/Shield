using Shield.Api.Models;

namespace Shield.Api.Services;

public sealed class PaymentFraudCheckRequestValidator : IPaymentFraudCheckRequestValidator
{
    public ValidationResult Validate(PaymentFraudCheckRequest? request)
    {
        if (request is null)
        {
            return ValidationResult.Invalid("Request payload is required.");
        }

        if (string.IsNullOrWhiteSpace(request.TransactionId))
        {
            return ValidationResult.Invalid("TransactionId must be provided.");
        }

        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            return ValidationResult.Invalid("UserId must be provided.");
        }

        if (string.IsNullOrWhiteSpace(request.Currency))
        {
            return ValidationResult.Invalid("Currency must be provided.");
        }

        if (request.Timestamp > DateTime.UtcNow)
        {
            return ValidationResult.Invalid("Timestamp cannot be in the future.");
        }

        if (request.Payer is null || string.IsNullOrWhiteSpace(request.Payer.AccountId))
        {
            return ValidationResult.Invalid("Payer.AccountId must be provided.");
        }

        if (request.Payee is null || string.IsNullOrWhiteSpace(request.Payee.AccountId))
        {
            return ValidationResult.Invalid("Payee.AccountId must be provided.");
        }

        if (request.Device is null || string.IsNullOrWhiteSpace(request.Device.DeviceId))
        {
            return ValidationResult.Invalid("Device.DeviceId must be provided.");
        }

        if (request.Network is null || string.IsNullOrWhiteSpace(request.Network.IpAddress))
        {
            return ValidationResult.Invalid("Network.IpAddress must be provided.");
        }

        if (request.AuthContext is { FailedLoginsLast24h: < 0 })
        {
            return ValidationResult.Invalid("AuthContext.FailedLoginsLast24h cannot be negative.");
        }

        return ValidationResult.Valid();
    }
}
