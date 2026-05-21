using domain.Events;

namespace domain.Entities;

public class Feature : Entity
{
    public Feature() { }
    public Feature(string name, string permission)
    {
        Id = Guid.NewGuid();
        Name = name;
        Permission = permission;

        AddDomainEvent(new FeatureCreatedEvent(Id, Name, Permission));
    }
    public string Name { get; private set; } = default!;
    public string Permission { get; private set; } = default!;

    public void UpdateFeature(string name, string permission)
    {
        Name = name;
        Permission = permission;
        AddDomainEvent(new FeatureUpdatedEvent(Id, Name, Permission));
    }
}