using domain.Entities;
using domain.Events;
using FluentAssertions;

namespace test_srp.Unit.Entities;

public class FeatureTests
{
    [Fact]
    public void Constructor_ShouldSetProperties_WhenValidParameters()
    {
        // Arrange
        var name = "Dashboard";
        var permission = "read:dashboard";

        // Act
        var feature = new Feature(name, permission);

        // Assert
        feature.Id.Should().NotBeEmpty();
        feature.Name.Should().Be(name);
        feature.Permission.Should().Be(permission);
    }

    [Fact]
    public void Constructor_ShouldAddDomainEvent_WhenCreated()
    {
        // Arrange & Act
        var feature = new Feature("Feature", "permission");

        // Assert
        feature.DomainEvents.Should().HaveCount(1);
        feature.DomainEvents.First().Should().BeOfType<FeatureCreatedEvent>();
    }

    [Fact]
    public void Constructor_ShouldCreateFeatureCreatedEvent_WithCorrectData()
    {
        // Arrange
        var name = "Dashboard";
        var permission = "read:dashboard";

        // Act
        var feature = new Feature(name, permission);

        // Assert
        var domainEvent = feature.DomainEvents.First() as FeatureCreatedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.Id.Should().Be(feature.Id);
        domainEvent.Name.Should().Be(name);
        domainEvent.Permission.Should().Be(permission);
    }

    [Fact]
    public void UpdateFeature_ShouldUpdateNameAndPermission()
    {
        // Arrange
        var feature = new Feature("Feature Original", "permission:original");
        var newName = "Feature Atualizada";
        var newPermission = "permission:updated";

        // Act
        feature.UpdateFeature(newName, newPermission);

        // Assert
        feature.Name.Should().Be(newName);
        feature.Permission.Should().Be(newPermission);
    }

    [Fact]
    public void UpdateFeature_ShouldAddDomainEvent()
    {
        // Arrange
        var feature = new Feature("Feature", "permission");
        feature.ClearDomainEvents();

        // Act
        feature.UpdateFeature("Novo Nome", "nova:permission");

        // Assert
        feature.DomainEvents.Should().HaveCount(1);
        feature.DomainEvents.First().Should().BeOfType<FeatureUpdatedEvent>();
    }
}
