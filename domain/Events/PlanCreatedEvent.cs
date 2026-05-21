
using MediatR;

namespace domain.Events;

public class PlanCreatedEvent(Guid id, string name, decimal price) : IDomainEvent, INotification
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
    public decimal Price { get; } = price;
    public DateTime OccurredIn { get; } = DateTime.UtcNow;
}

public class PlanUpdatedEvent(Guid id, string name, decimal price) : IDomainEvent, INotification
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
    public decimal Price { get; } = price;
    public DateTime OccurredIn { get; } = DateTime.UtcNow;
}