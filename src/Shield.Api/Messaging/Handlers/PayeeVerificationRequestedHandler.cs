using Shield.Api.Messaging.Events;
using Shield.Api.Services.Downstream;

namespace Shield.Api.Messaging.Handlers;

public sealed class PayeeVerificationRequestedHandler(
    INotificationService notificationService,
    ILogger<PayeeVerificationRequestedHandler> logger)
{
    public async Task Handle(PayeeVerificationRequested message, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Handling payee verification notification for transaction {TransactionId} targeting {PayeeAccountId} due to {Reason}.",
            message.TransactionId,
            message.PayeeAccountId,
            message.Reason);

        await notificationService.NotifyPayeeVerificationAsync(message, cancellationToken);
    }
}
