using Shield.Api.Models;
using Shield.Api.Services;

namespace Shield.Api.UnitTests.Services;

[TestClass]
public sealed class PaymentFraudCheckRequestValidatorTests
{
    private PaymentFraudCheckRequestValidator _systemUnderTest;

    [TestInitialize]
    public void Setup()
    {
        _systemUnderTest = new PaymentFraudCheckRequestValidator();
    }

    [TestMethod]
    public void Validate_WhenRequestIsNull_ReturnsInvalidWithExpectedMessage()
    {
        var result = _systemUnderTest.Validate(null);

        Assert.IsFalse(result.IsValid);
        Assert.AreEqual("Request payload is required.", result.ErrorMessage);
    }

    [TestMethod]
    public void Validate_WhenRequestIsValid_ReturnsValid()
    {
        var request = CreateRequest();

        var result = _systemUnderTest.Validate(request);

        Assert.IsTrue(result.IsValid);
        Assert.IsNull(result.ErrorMessage);
    }

    [TestMethod]
    public void Validate_WhenTransactionIdMissing_ReturnsInvalid()
    {
        var request = CreateRequest(transactionId: string.Empty);

        var result = _systemUnderTest.Validate(request);

        Assert.IsFalse(result.IsValid);
        Assert.AreEqual("TransactionId must be provided.", result.ErrorMessage);
    }

    [TestMethod]
    public void Validate_WhenFailedLoginsNegative_ReturnsInvalid()
    {
        var request = CreateRequest(failedLogins: -1);

        var result = _systemUnderTest.Validate(request);

        Assert.IsFalse(result.IsValid);
        Assert.AreEqual("AuthContext.FailedLoginsLast24h cannot be negative.", result.ErrorMessage);
    }

    [TestMethod]
    public void Validate_WhenTimestampInFuture_ReturnsInvalid()
    {
        var request = CreateRequest(timestamp: DateTime.UtcNow.AddMinutes(5));

        var result = _systemUnderTest.Validate(request);

        Assert.IsFalse(result.IsValid);
        Assert.AreEqual("Timestamp cannot be in the future.", result.ErrorMessage);
    }

    private static PaymentFraudCheckRequest CreateRequest(
        string transactionId = "txn-123",
        string userId = "user-123",
        decimal amount = 100m,
        string currency = "GBP",
        DateTime? timestamp = null,
        int failedLogins = 0)
    {
        var effectiveTimestamp = timestamp ?? DateTime.UtcNow.AddMinutes(-1);

        return new PaymentFraudCheckRequest
        {
            TransactionId = transactionId,
            UserId = userId,
            Amount = amount,
            Currency = currency,
            Timestamp = effectiveTimestamp,
            Payer = new PayerDetails
            {
                AccountId = "payer-1",
                Country = "GB"
            },
            Payee = new PayeeDetails
            {
                AccountId = "payee-1",
                IsNewPayee = false,
                MerchantCategory = PaymentMerchantCategory.Unknown
            },
            Device = new DeviceDetails
            {
                DeviceId = "device-1",
                IsNewDevice = false
            },
            Network = new NetworkDetails
            {
                IpAddress = "127.0.0.1",
                Country = "GB"
            },
            AuthContext = new AuthContextDetails
            {
                FailedLoginsLast24h = failedLogins,
                LastLoginCountry = "GB"
            }
        };
    }
}
