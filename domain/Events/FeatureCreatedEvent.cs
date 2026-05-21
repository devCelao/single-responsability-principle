using MediatR;

namespace domain.Events;

public class FeatureCreatedEvent(Guid id, string name, string permission) : IDomainEvent, INotification
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
    public string Permission { get; } = permission;
    public DateTime OccurredIn { get; } = DateTime.UtcNow;
}

public class FeatureUpdatedEvent(Guid id, string name, string permission) : IDomainEvent, INotification
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
    public string Permission { get; } = permission;
    public DateTime OccurredIn { get; } = DateTime.UtcNow;
}