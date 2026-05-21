using domain.Entities;
using domain.Events;
using FluentAssertions;

namespace test_srp.Unit.Entities;

public class PlanTests
{
    [Fact]
    public void Constructor_ShouldSetProperties_WhenValidParameters()
    {
        // Arrange
        var name = "Plano Premium";
        var description = "Descricao do plano";
        var price = 199.90m;

        // Act
        var plan = new Plan(name, description, price);

        // Assert
        plan.Id.Should().NotBeEmpty();
        plan.Name.Should().Be(name);
        plan.Description.Should().Be(description);
        plan.Price.Should().Be(price);
        plan.IsActive.Should().BeFalse();
        plan.Services.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_ShouldAddDomainEvent_WhenCreated()
    {
        // Arrange & Act
        var plan = new Plan("Plano", "Descricao", 100m);

        // Assert
        plan.DomainEvents.Should().HaveCount(1);
        plan.DomainEvents.First().Should().BeOfType<PlanCreatedEvent>();
    }

    [Fact]
    public void Constructor_ShouldCreatePlanCreatedEvent_WithCorrectData()
    {
        // Arrange
        var name = "Plano Premium";
        var price = 199.90m;

        // Act
        var plan = new Plan(name, "Descricao", price);

        // Assert
        var domainEvent = plan.DomainEvents.First() as PlanCreatedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.Id.Should().Be(plan.Id);
        domainEvent.Name.Should().Be(name);
        domainEvent.Price.Should().Be(price);
    }

    [Fact]
    public void Activate_ShouldSetIsActiveTrue()
    {
        // Arrange
        var plan = new Plan("Plano", "Descricao", 100m);
        plan.IsActive.Should().BeFalse();

        // Act
        plan.Activate();

        // Assert
        plan.IsActive.Should().BeTrue();
    }

    [Fact]
    public void UpdatePlan_ShouldUpdateAllProperties()
    {
        // Arrange
        var plan = new Plan("Plano Original", "Descricao Original", 100m);
        var newName = "Plano Atualizado";
        var newDescription = "Descricao Atualizada";
        var newPrice = 200m;
        var newIsActive = true;

        // Act
        plan.UpdatePlan(newName, newDescription, newPrice, newIsActive);

        // Assert
        plan.Name.Should().Be(newName);
        plan.Description.Should().Be(newDescription);
        plan.Price.Should().Be(newPrice);
        plan.IsActive.Should().Be(newIsActive);
    }

    [Fact]
    public void UpdatePlan_ShouldAddDomainEvent()
    {
        // Arrange
        var plan = new Plan("Plano", "Descricao", 100m);
        plan.ClearDomainEvents();

        // Act
        plan.UpdatePlan("Novo Nome", "Nova Descricao", 150m, true);

        // Assert
        plan.DomainEvents.Should().HaveCount(1);
        plan.DomainEvents.First().Should().BeOfType<PlanUpdatedEvent>();
    }

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        var plan = new Plan("Plano", "Descricao", 100m);
        plan.DomainEvents.Should().NotBeEmpty();

        // Act
        plan.ClearDomainEvents();

        // Assert
        plan.DomainEvents.Should().BeEmpty();
    }
}
