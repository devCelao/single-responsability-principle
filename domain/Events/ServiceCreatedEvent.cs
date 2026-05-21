using MediatR;

namespace domain.Events;

public class ServiceCreatedEvent(Guid id, string name, string type) : IDomainEvent, INotification
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
    public string Type { get; } = type;
    public DateTime OccurredIn { get; } = DateTime.UtcNow;
}

public class ServiceUpdatedEvent(Guid id, string name, string type) : IDomainEvent, INotification
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
    public string Type { get; } = type;
    public DateTime OccurredIn { get; } = DateTime.UtcNow;
}