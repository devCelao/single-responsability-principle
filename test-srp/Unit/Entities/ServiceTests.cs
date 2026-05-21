using domain.Entities;
using domain.Events;
using FluentAssertions;

namespace test_srp.Unit.Entities;

public class ServiceTests
{
    [Fact]
    public void Constructor_ShouldSetProperties_WhenValidParameters()
    {
        // Arrange
        var name = "API Gateway";
        var type = "Infrastructure";

        // Act
        var service = new Service(name, type);

        // Assert
        service.Id.Should().NotBeEmpty();
        service.Name.Should().Be(name);
        service.Type.Should().Be(type);
        service.IsActive.Should().BeFalse();
        service.Features.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_ShouldAddDomainEvent_WhenCreated()
    {
        // Arrange & Act
        var service = new Service("Servico", "Tipo");

        // Assert
        service.DomainEvents.Should().HaveCount(1);
        service.DomainEvents.First().Should().BeOfType<ServiceCreatedEvent>();
    }

    [Fact]
    public void Constructor_ShouldCreateServiceCreatedEvent_WithCorrectData()
    {
        // Arrange
        var name = "API Gateway";
        var type = "Infrastructure";

        // Act
        var service = new Service(name, type);

        // Assert
        var domainEvent = service.DomainEvents.First() as ServiceCreatedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.Id.Should().Be(service.Id);
        domainEvent.Name.Should().Be(name);
        domainEvent.Type.Should().Be(type);
    }

    [Fact]
    public void Activate_ShouldSetIsActiveTrue()
    {
        // Arrange
        var service = new Service("Servico", "Tipo");
        service.IsActive.Should().BeFalse();

        // Act
        service.Activate();

        // Assert
        service.IsActive.Should().BeTrue();
    }

    [Fact]
    public void UpdateService_ShouldUpdateAllProperties()
    {
        // Arrange
        var service = new Service("Servico Original", "Tipo Original");
        var newName = "Servico Atualizado";
        var newType = "Tipo Atualizado";
        var newIsActive = true;

        // Act
        service.UpdateService(newName, newType, newIsActive);

        // Assert
        service.Name.Should().Be(newName);
        service.Type.Should().Be(newType);
        service.IsActive.Should().Be(newIsActive);
    }

    [Fact]
    public void UpdateService_ShouldAddDomainEvent()
    {
        // Arrange
        var service = new Service("Servico", "Tipo");
        service.ClearDomainEvents();

        // Act
        service.UpdateService("Novo Nome", "Novo Tipo", true);

        // Assert
        service.DomainEvents.Should().HaveCount(1);
        service.DomainEvents.First().Should().BeOfType<ServiceUpdatedEvent>();
    }
}
