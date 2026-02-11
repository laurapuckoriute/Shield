using Shield.Api.Messaging.Events;

namespace Shield.Api.Services.Downstream;

public sealed class NotificationService(ILogger<NotificationService> logger) : INotificationService
{
    public Task NotifyPayeeVerificationAsync(PayeeVerificationRequested notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Payee verification notification dispatched for transaction {TransactionId} targeting account {PayeeAccountId}.",
            notification.TransactionId,
            notification.PayeeAccountId);

        return Task.CompletedTask;
    }
}
