using application.Queries;
using domain.Repositories;
using FluentAssertions;
using Moq;
using test_srp.Helpers;

namespace test_srp.Unit.Queries;

public class ServiceQueryServiceTests
{
    private readonly Mock<IServiceRepository> _serviceRepositoryMock;
    private readonly ServiceQueryService _sut;

    public ServiceQueryServiceTests()
    {
        _serviceRepositoryMock = new Mock<IServiceRepository>();
        _sut = new ServiceQueryService(_serviceRepositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingService_ReturnsDto()
    {
        // Arrange
        var service = TestDataBuilder.CreateService(
            name: "API Gateway",
            type: "Infrastructure");
        var serviceId = service.Id;

        _serviceRepositoryMock
            .Setup(r => r.GetServiceByIdAsync(serviceId))
            .ReturnsAsync(service);

        // Act
        var result = await _sut.GetByIdAsync(serviceId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(serviceId);
        result.Name.Should().Be("API Gateway");
        result.Type.Should().Be("Infrastructure");
    }

    [Fact]
    public async Task GetByIdAsync_CallsRepository()
    {
        // Arrange
        var serviceId = Guid.NewGuid();
        var service = TestDataBuilder.CreateService();

        _serviceRepositoryMock
            .Setup(r => r.GetServiceByIdAsync(serviceId))
            .ReturnsAsync(service);

        // Act
        await _sut.GetByIdAsync(serviceId);

        // Assert
        _serviceRepositoryMock.Verify(r => r.GetServiceByIdAsync(serviceId), Times.Once);
    }

    [Fact]
    public async Task GetByTypeAsync_WithMatchingServices_ReturnsFilteredServices()
    {
        // Arrange
        var services = new List<domain.Entities.Service>
        {
            TestDataBuilder.CreateService(name: "Service 1", type: "Premium"),
            TestDataBuilder.CreateService(name: "Service 2", type: "Premium")
        };

        _serviceRepositoryMock
            .Setup(r => r.GetByTypeAsync("Premium"))
            .ReturnsAsync(services);

        // Act
        var result = await _sut.GetByTypeAsync("Premium");

        // Assert
        result.Should().HaveCount(2);
        result.All(s => s.Type == "Premium").Should().BeTrue();
    }

    [Fact]
    public async Task GetByTypeAsync_WithNoMatchingServices_ReturnsEmptyCollection()
    {
        // Arrange
        _serviceRepositoryMock
            .Setup(r => r.GetByTypeAsync("NonExistent"))
            .ReturnsAsync(new List<domain.Entities.Service>());

        // Act
        var result = await _sut.GetByTypeAsync("NonExistent");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByTypeAsync_CallsRepository()
    {
        // Arrange
        var type = "Premium";

        _serviceRepositoryMock
            .Setup(r => r.GetByTypeAsync(type))
            .ReturnsAsync(new List<domain.Entities.Service>());

        // Act
        await _sut.GetByTypeAsync(type);

        // Assert
        _serviceRepositoryMock.Verify(r => r.GetByTypeAsync(type), Times.Once);
    }
}
