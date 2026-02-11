using Shield.Api.Messaging.Events;

namespace Shield.Api.Services.Downstream;

public interface IHumanReviewService
{
    Task RequestReviewAsync(HumanInterventionRequested notification, CancellationToken cancellationToken = default);
}
