using domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace application.EventHandlers;

public class ServiceCreatedEventHandler(ILogger<ServiceCreatedEventHandler> logger) : INotificationHandler<ServiceCreatedEvent>
{
    public async Task Handle(ServiceCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation(
          "Service created. ID: {ServiceId} | Name: {ServiceName} | Type: {ServiceType} | Date: {OccurredIn}",
          domainEvent.Id,
          domainEvent.Name,
          domainEvent.Type,
          domainEvent.OccurredIn
      );
        await Task.CompletedTask;
    }
}
