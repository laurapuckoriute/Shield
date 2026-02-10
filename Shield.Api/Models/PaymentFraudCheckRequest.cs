namespace Shield.Api.Models;

public sealed class PaymentFraudCheckRequest
{
    public string TransactionId { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public PayerDetails Payer { get; init; } = new();
    public PayeeDetails Payee { get; init; } = new();
    public DeviceDetails Device { get; init; } = new();
    public NetworkDetails Network { get; init; } = new();
    public AuthContextDetails AuthContext { get; init; } = new();
}

public sealed class PayerDetails
{
    public string AccountId { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
}

public sealed class PayeeDetails
{
    public string AccountId { get; init; } = string.Empty;
    public bool IsNewPayee { get; init; }
    public PaymentMerchantCategory MerchantCategory { get; init; } = PaymentMerchantCategory.Unknown;
}

public sealed class DeviceDetails
{
    public string DeviceId { get; init; } = string.Empty;
    public bool IsNewDevice { get; init; }
}

public sealed class NetworkDetails
{
    public string IpAddress { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
}

public sealed class AuthContextDetails
{
    public int FailedLoginsLast24h { get; init; }
    public string LastLoginCountry { get; init; } = string.Empty;
}
