using application.Queries;
using domain.Repositories;
using FluentAssertions;
using Moq;
using test_srp.Helpers;

namespace test_srp.Unit.Queries;

public class FeatureQueryServiceTests
{
    private readonly Mock<IFeatureRepository> _featureRepositoryMock;
    private readonly FeatureQueryService _sut;

    public FeatureQueryServiceTests()
    {
        _featureRepositoryMock = new Mock<IFeatureRepository>();
        _sut = new FeatureQueryService(_featureRepositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingFeature_ReturnsDto()
    {
        // Arrange
        var feature = TestDataBuilder.CreateFeature(
            name: "Dashboard",
            permission: "read:dashboard");
        var featureId = feature.Id;

        _featureRepositoryMock
            .Setup(r => r.GetFeatureByIdAsync(featureId))
            .ReturnsAsync(feature);

        // Act
        var result = await _sut.GetByIdAsync(featureId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(featureId);
        result.Name.Should().Be("Dashboard");
        result.Permission.Should().Be("read:dashboard");
    }

    [Fact]
    public async Task GetByIdAsync_CallsRepository()
    {
        // Arrange
        var featureId = Guid.NewGuid();
        var feature = TestDataBuilder.CreateFeature();

        _featureRepositoryMock
            .Setup(r => r.GetFeatureByIdAsync(featureId))
            .ReturnsAsync(feature);

        // Act
        await _sut.GetByIdAsync(featureId);

        // Assert
        _featureRepositoryMock.Verify(r => r.GetFeatureByIdAsync(featureId), Times.Once);
    }

    [Fact]
    public async Task GetFeatureByNameAsync_ExistingFeature_ReturnsDto()
    {
        // Arrange
        var feature = TestDataBuilder.CreateFeature(
            name: "Dashboard",
            permission: "read:dashboard");

        _featureRepositoryMock
            .Setup(r => r.GetFeatureByNameAsync("Dashboard"))
            .ReturnsAsync(feature);

        // Act
        var result = await _sut.GetFeatureByNameAsync("Dashboard");

        // Assert
        result.Should().NotBeNull();
        result?.Name.Should().Be("Dashboard");
        result?.Permission.Should().Be("read:dashboard");
    }

    [Fact]
    public async Task GetFeatureByNameAsync_CallsRepository()
    {
        // Arrange
        var featureName = "Dashboard";
        var feature = TestDataBuilder.CreateFeature(name: featureName);

        _featureRepositoryMock
            .Setup(r => r.GetFeatureByNameAsync(featureName))
            .ReturnsAsync(feature);

        // Act
        await _sut.GetFeatureByNameAsync(featureName);

        // Assert
        _featureRepositoryMock.Verify(r => r.GetFeatureByNameAsync(featureName), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_MapsAllProperties()
    {
        // Arrange
        var feature = TestDataBuilder.CreateFeature(
            name: "Reports",
            permission: "admin:reports");
        var featureId = feature.Id;

        _featureRepositoryMock
            .Setup(r => r.GetFeatureByIdAsync(featureId))
            .ReturnsAsync(feature);

        // Act
        var result = await _sut.GetByIdAsync(featureId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(featureId);
        result.Name.Should().Be("Reports");
        result.Permission.Should().Be("admin:reports");
    }
}
