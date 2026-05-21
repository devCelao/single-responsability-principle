using domain.Events;
using MediatR;

namespace infrastructure.Events;

public class MediatRDomainEventDispatcher(IMediator mediator) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IEnumerable<IDomainEvent> events)
    {
        foreach (var domainEvent in events)
        {
            if (domainEvent is INotification notification)
                await mediator.Publish(notification);
        }
    }
}
