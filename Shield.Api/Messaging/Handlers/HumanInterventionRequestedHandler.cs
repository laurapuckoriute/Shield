using Shield.Api.Messaging.Events;
using Shield.Api.Services;

namespace Shield.Api.Messaging.Handlers;

public sealed class HumanInterventionRequestedHandler(
    IHumanReviewService reviewService,
    ILogger<HumanInterventionRequestedHandler> logger)
{
    public async Task Handle(HumanInterventionRequested message, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Handling human intervention request for transaction {TransactionId} due to {Reason}.",
            message.TransactionId,
            message.Reason);

        await reviewService.RequestReviewAsync(message, cancellationToken);
    }
}
