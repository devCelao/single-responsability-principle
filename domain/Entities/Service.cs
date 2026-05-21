using domain.Events;

namespace domain.Entities;

public class Service : Entity
{
    public Service() { }
    public Service(string name, string type)
    {
        Id = Guid.NewGuid();
        Name = name;
        Type = type;
        IsActive = false;
        AddDomainEvent(new ServiceCreatedEvent(Id, Name, Type));
    }
    public string Name { get; private set; } = default!;
    public string Type { get; private set; } = default!;
    public bool IsActive { get; private set; }
    public List<Feature> Features { get; private set; } = [];
    public void Activate() => IsActive = true;

    public void UpdateService(string name, string type, bool isActive)
    {
        Name = name;
        Type = type;
        IsActive = isActive;
        AddDomainEvent(new ServiceUpdatedEvent(Id, Name, Type));
    }
}
