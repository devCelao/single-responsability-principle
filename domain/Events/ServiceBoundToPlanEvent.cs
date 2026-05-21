namespace domain.Events;

public class ServiceBoundToPlanEvent(Guid planId, Guid serviceId) : IDomainEvent
{
    public Guid PlanId { get; } = planId;
    public Guid ServiceId { get; } = serviceId;
    public DateTime OccurredIn { get; } = DateTime.UtcNow;
}
