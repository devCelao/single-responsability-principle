using application.Dtos;
using application.UseCases.Service;
using domain.Repositories;
using FluentAssertions;
using Moq;
using test_srp.Helpers;

namespace test_srp.Unit.UseCases.Service;

public class UpdateServiceUseCaseTests
{
    private readonly Mock<IServiceRepository> _serviceRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateServiceUseCase _sut;

    public UpdateServiceUseCaseTests()
    {
        _serviceRepositoryMock = new Mock<IServiceRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new UpdateServiceUseCase(_serviceRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ExistingService_UpdatesAndReturnsId()
    {
        // Arrange
        var existingService = TestDataBuilder.CreateService();
        var serviceId = existingService.Id;
        var updateDto = TestDataBuilder.CreateValidServiceDto(
            name: "Updated Name",
            type: "Updated Type");

        _serviceRepositoryMock
            .Setup(r => r.GetServiceByIdAsync(serviceId))
            .ReturnsAsync(existingService);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        var result = await _sut.ExecuteAsync(serviceId, updateDto);

        // Assert
        result.Should().Be(serviceId);
    }

    [Fact]
    public async Task ExecuteAsync_ExistingService_CallsUpdateService()
    {
        // Arrange
        var existingService = TestDataBuilder.CreateService();
        var serviceId = existingService.Id;
        var updateDto = TestDataBuilder.CreateValidServiceDto();

        _serviceRepositoryMock
            .Setup(r => r.GetServiceByIdAsync(serviceId))
            .ReturnsAsync(existingService);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        await _sut.ExecuteAsync(serviceId, updateDto);

        // Assert
        _serviceRepositoryMock.Verify(r => r.UpdateService(It.IsAny<domain.Entities.Service>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ExistingService_UpdatesServiceProperties()
    {
        // Arrange
        var existingService = TestDataBuilder.CreateService();
        var serviceId = existingService.Id;
        var updateDto = TestDataBuilder.CreateValidServiceDto(
            name: "New Name",
            type: "New Type");

        _serviceRepositoryMock
            .Setup(r => r.GetServiceByIdAsync(serviceId))
            .ReturnsAsync(existingService);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        await _sut.ExecuteAsync(serviceId, updateDto);

        // Assert
        existingService.Name.Should().Be("New Name");
        existingService.Type.Should().Be("New Type");
    }

    [Fact]
    public async Task ExecuteAsync_ServiceNotFound_ThrowsException()
    {
        // Arrange
        var serviceId = Guid.NewGuid();
        var updateDto = TestDataBuilder.CreateValidServiceDto();

        _serviceRepositoryMock
            .Setup(r => r.GetServiceByIdAsync(serviceId))
            .ReturnsAsync((domain.Entities.Service?)null);

        // Act
        var act = () => _sut.ExecuteAsync(serviceId, updateDto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Service not found");
    }

    [Fact]
    public async Task ExecuteAsync_CommitFails_ThrowsException()
    {
        // Arrange
        var existingService = TestDataBuilder.CreateService();
        var serviceId = existingService.Id;
        var updateDto = TestDataBuilder.CreateValidServiceDto();

        _serviceRepositoryMock
            .Setup(r => r.GetServiceByIdAsync(serviceId))
            .ReturnsAsync(existingService);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(false);

        // Act
        var act = () => _sut.ExecuteAsync(serviceId, updateDto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Failed to update the service.");
    }
}
