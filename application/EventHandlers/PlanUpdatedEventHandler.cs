using domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace application.EventHandlers;

public class PlanUpdatedEventHandler(ILogger<PlanUpdatedEventHandler> logger) : INotificationHandler<PlanUpdatedEvent>
{
    public async Task Handle(PlanUpdatedEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Plan updated successfully. ID: {PlanId} | Name: {PlanName} | Price: R$ {PlanPrice:N2} | Date: {OccurredIn}",
            domainEvent.Id,
            domainEvent.Name,
            domainEvent.Price,
            domainEvent.OccurredIn
        );
        await Task.CompletedTask;
    }
}