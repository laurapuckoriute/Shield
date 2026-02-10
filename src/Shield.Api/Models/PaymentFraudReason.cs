namespace Shield.Api.Models;

public enum PaymentFraudReason
{
    Unknown = 0,
    HighTransactionAmount,
    IpCountryMismatch,
    NewPayee,
    NewDevice,
    RecentFailedLogins,
    HighRiskMerchantCategory
}
