using domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace application.EventHandlers;

public class FeatureCreatedEventHandler(ILogger<FeatureCreatedEventHandler> logger) : INotificationHandler<FeatureCreatedEvent>
{
    public async Task Handle(FeatureCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Feature created. ID: {FeatureId} | Name: {FeatureName} | Permission: {Permission} | Date: {OccurredIn}",
            domainEvent.Id,
            domainEvent.Name,
            domainEvent.Permission,
            domainEvent.OccurredIn
        );

        await Task.CompletedTask;
    }
}
