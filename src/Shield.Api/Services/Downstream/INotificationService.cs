using Shield.Api.Messaging.Events;

namespace Shield.Api.Services.Downstream;

public interface INotificationService
{
    Task NotifyPayeeVerificationAsync(PayeeVerificationRequested notification, CancellationToken cancellationToken = default);
}
