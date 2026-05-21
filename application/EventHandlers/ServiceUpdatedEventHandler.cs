using domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace application.EventHandlers;

public class ServiceUpdatedEventHandler(ILogger<ServiceUpdatedEventHandler> logger) : INotificationHandler<ServiceUpdatedEvent>
{
    public async Task Handle(ServiceUpdatedEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Service updated successfully. ID: {ServiceId} | Name: {ServiceName} | Type: {ServiceType} | Date: {OccurredIn}",
            domainEvent.Id,
            domainEvent.Name,
            domainEvent.Type,
            domainEvent.OccurredIn
        );
        await Task.CompletedTask;
    }
}