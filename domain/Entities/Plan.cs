using domain.Events;

namespace domain.Entities;

public class Plan : Entity
{
    public Plan() { }
    public Plan(string name, string description, decimal price)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Price = price;
        IsActive = false;

        AddDomainEvent(new PlanCreatedEvent(Id, Name, Price));
    }
   
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public decimal Price { get; private set; }
    public bool IsActive { get; private set; }
    public List<Service> Services { get; private set; } = [];
    
    
    public void Activate() => IsActive = true;
    

    public void UpdatePlan(string name, string description, decimal price, bool isActive)
    {
        Name = name;
        Description = description;
        Price = price;
        IsActive = isActive;
        AddDomainEvent(new PlanUpdatedEvent(Id, Name, Price));
    }
}