using Shield.Api.Messaging.Events;

namespace Shield.Api.Services;

public sealed class HumanReviewService(ILogger<HumanReviewService> logger) : IHumanReviewService
{
    public Task RequestReviewAsync(HumanInterventionRequested notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Human intervention requested for transaction {TransactionId} (failed logins: {FailedLogins}).",
            notification.TransactionId,
            notification.FailedLoginsLast24h);

        return Task.CompletedTask;
    }
}
