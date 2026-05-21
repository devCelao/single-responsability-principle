using domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace application.EventHandlers;

public class PlanCreatedEventHandler(ILogger<PlanCreatedEventHandler> logger) : INotificationHandler<PlanCreatedEvent>
{
    //private readonly IMessageBus _messageBus; // RabbitMQ, Azure Service Bus, etc.
    public async Task Handle(PlanCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        //await _messageBus.PublishAsync("plan.created", new
        //{
        //    domainEvent.Id,
        //    domainEvent.Name,
        //    domainEvent.Price,
        //    domainEvent.OccurredIn
        //});
        
        logger.LogInformation(
           "Plan created. ID: {PlanId} | Name: {PlanName} | Price: R$ {PlanPrice:N2} | Date: {OccurredIn}",
           domainEvent.Id,
           domainEvent.Name,
           domainEvent.Price,
           domainEvent.OccurredIn
       );
        await Task.CompletedTask;
    }
}
