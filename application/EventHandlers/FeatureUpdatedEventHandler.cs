using domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace application.EventHandlers;

public class FeatureUpdatedEventHandler(ILogger<FeatureUpdatedEventHandler> logger) : INotificationHandler<FeatureUpdatedEvent>
{
    public async Task Handle(FeatureUpdatedEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Feature updated successfully. ID: {FeatureId} | Name: {FeatureName} | Permission: {Permission} | Date: {OccurredIn}",
            domainEvent.Id,
            domainEvent.Name,
            domainEvent.Permission,
            domainEvent.OccurredIn
        );

        await Task.CompletedTask;
    }
}