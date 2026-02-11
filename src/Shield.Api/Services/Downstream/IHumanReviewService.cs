using Shield.Api.Messaging.Events;

namespace Shield.Api.Services;

public interface IHumanReviewService
{
    Task RequestReviewAsync(HumanInterventionRequested notification, CancellationToken cancellationToken = default);
}
