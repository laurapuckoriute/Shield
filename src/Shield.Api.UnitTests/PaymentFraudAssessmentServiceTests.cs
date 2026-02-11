using Microsoft.Extensions.Logging;
using Moq;
using Shield.Api.Messaging.Events;
using Shield.Api.Models;
using Shield.Api.Services;
using Wolverine;

namespace Shield.Api.UnitTests;

[TestClass]
public sealed class PaymentFraudAssessmentServiceTests
{
    private Mock<IMessageContext> _messageContextMock;
    private Mock<ILogger<PaymentFraudAssessmentService>> _loggerMock;
    private PaymentFraudAssessmentService _serviceUnderTest;

    [TestInitialize]
    public void Setup()
    {
        _messageContextMock = new Mock<IMessageContext>();
        _loggerMock = new Mock<ILogger<PaymentFraudAssessmentService>>();
        
        _serviceUnderTest = new PaymentFraudAssessmentService(_messageContextMock.Object, _loggerMock.Object);
        
        _messageContextMock
            .Setup(m => m.PublishAsync(It.IsAny<HumanInterventionRequested>(), It.IsAny<DeliveryOptions?>()))
            .Returns(ValueTask.CompletedTask);

        _messageContextMock
            .Setup(m => m.PublishAsync(It.IsAny<PayeeVerificationRequested>(), It.IsAny<DeliveryOptions?>()))
            .Returns(ValueTask.CompletedTask);
    }

    [TestMethod]
    public async Task Assess_WhenRiskScoreExceedsThreshold_ReturnsLikelyFraudWithReasons()
    {
        var request = CreateRequest(amount: 6000m, newPayee: true);

        var result = await _serviceUnderTest.Assess(request);

        Assert.IsTrue(result.IsLikelyFraud);
        Assert.AreEqual(0.6, result.RiskScore);
        CollectionAssert.AreEquivalent(
            new[]
            {
                PaymentFraudReason.HighTransactionAmount,
                PaymentFraudReason.NewPayee
            },
            result.Reasons.ToList());
    }

    [TestMethod]
    public async Task Assess_WhenRiskScoreBelowThreshold_ReturnsNotLikelyFraudAndDoesNotPublish()
    {
        var request = CreateRequest();

        var result = await _serviceUnderTest.Assess(request);

        Assert.IsFalse(result.IsLikelyFraud);
        Assert.AreEqual(0.0, result.RiskScore);
        Assert.AreEqual(0, result.Reasons.Count);
        Assert.AreEqual(0, _messageContextMock.Invocations.Count);
    }

    [TestMethod]
    public async Task Assess_WhenRecentFailedLogins_PublishesHumanInterventionEvent()
    {
        var request = CreateRequest(failedLogins: 2);

        await _serviceUnderTest.Assess(request);

        var publishedEvent = _messageContextMock.Invocations
            .Select(invocation => invocation.Arguments.FirstOrDefault())
            .OfType<HumanInterventionRequested>()
            .Single();

        Assert.AreEqual(request.TransactionId, publishedEvent.TransactionId);
        Assert.AreEqual(request.UserId, publishedEvent.UserId);
        Assert.AreEqual(request.Amount, publishedEvent.Amount);
        Assert.AreEqual(request.Currency, publishedEvent.Currency);
        Assert.AreEqual(request.Timestamp, publishedEvent.Timestamp);
        Assert.AreEqual(request.AuthContext.FailedLoginsLast24h, publishedEvent.FailedLoginsLast24h);
        Assert.AreEqual(PaymentFraudReason.RecentFailedLogins, publishedEvent.Reason);
    }

    [TestMethod]
    public async Task Assess_WhenNewPayee_PublishesPayeeVerificationEvent()
    {
        var request = CreateRequest(newPayee: true, merchantCategory: PaymentMerchantCategory.Crypto);

        await _serviceUnderTest.Assess(request);

        var publishedEvent = _messageContextMock.Invocations
            .Select(invocation => invocation.Arguments.FirstOrDefault())
            .OfType<PayeeVerificationRequested>()
            .Single();

        Assert.AreEqual(request.TransactionId, publishedEvent.TransactionId);
        Assert.AreEqual(request.UserId, publishedEvent.UserId);
        Assert.AreEqual(request.Payee.AccountId, publishedEvent.PayeeAccountId);
        Assert.AreEqual(request.Payee.MerchantCategory, publishedEvent.MerchantCategory);
        Assert.AreEqual(request.Amount, publishedEvent.Amount);
        Assert.AreEqual(request.Currency, publishedEvent.Currency);
        Assert.AreEqual(request.Timestamp, publishedEvent.Timestamp);
        Assert.AreEqual(PaymentFraudReason.NewPayee, publishedEvent.Reason);
    }

    private static PaymentFraudCheckRequest CreateRequest(
        decimal amount = 1000m,
        bool newPayee = false,
        bool newDevice = false,
        int failedLogins = 0,
        PaymentMerchantCategory merchantCategory = PaymentMerchantCategory.Unknown,
        string payerCountry = "GB",
        string networkCountry = "GB")
    {
        return new PaymentFraudCheckRequest
        {
            TransactionId = "txn-123",
            UserId = "user-123",
            Amount = amount,
            Currency = "GBP",
            Timestamp = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Payer = new PayerDetails
            {
                AccountId = "payer-1",
                Country = payerCountry
            },
            Payee = new PayeeDetails
            {
                AccountId = "payee-1",
                IsNewPayee = newPayee,
                MerchantCategory = merchantCategory
            },
            Device = new DeviceDetails
            {
                DeviceId = "device-1",
                IsNewDevice = newDevice
            },
            Network = new NetworkDetails
            {
                IpAddress = "127.0.0.1",
                Country = networkCountry
            },
            AuthContext = new AuthContextDetails
            {
                FailedLoginsLast24h = failedLogins,
                LastLoginCountry = payerCountry
            }
        };
    }
}
