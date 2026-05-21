using application.UseCases.Service;
using domain.Repositories;
using FluentAssertions;
using Moq;
using test_srp.Helpers;

namespace test_srp.Unit.UseCases.Service;

public class DeleteServiceUseCaseTests
{
    private readonly Mock<IServiceRepository> _serviceRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DeleteServiceUseCase _sut;

    public DeleteServiceUseCaseTests()
    {
        _serviceRepositoryMock = new Mock<IServiceRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new DeleteServiceUseCase(_serviceRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ExistingService_DeletesSuccessfully()
    {
        // Arrange
        var existingService = TestDataBuilder.CreateService();
        var serviceId = existingService.Id;

        _serviceRepositoryMock
            .Setup(r => r.GetServiceByIdAsync(serviceId))
            .ReturnsAsync(existingService);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        var act = () => _sut.ExecuteAsync(serviceId);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ExecuteAsync_ExistingService_CallsDeleteServiceAsync()
    {
        // Arrange
        var existingService = TestDataBuilder.CreateService();
        var serviceId = existingService.Id;

        _serviceRepositoryMock
            .Setup(r => r.GetServiceByIdAsync(serviceId))
            .ReturnsAsync(existingService);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        await _sut.ExecuteAsync(serviceId);

        // Assert
        _serviceRepositoryMock.Verify(r => r.DeleteServiceAsync(existingService), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ExistingService_CallsCommitAsync()
    {
        // Arrange
        var existingService = TestDataBuilder.CreateService();
        var serviceId = existingService.Id;

        _serviceRepositoryMock
            .Setup(r => r.GetServiceByIdAsync(serviceId))
            .ReturnsAsync(existingService);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        await _sut.ExecuteAsync(serviceId);

        // Assert
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ServiceNotFound_ThrowsException()
    {
        // Arrange
        var serviceId = Guid.NewGuid();

        _serviceRepositoryMock
            .Setup(r => r.GetServiceByIdAsync(serviceId))
            .ReturnsAsync((domain.Entities.Service?)null);

        // Act
        var act = () => _sut.ExecuteAsync(serviceId);

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

        _serviceRepositoryMock
            .Setup(r => r.GetServiceByIdAsync(serviceId))
            .ReturnsAsync(existingService);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(false);

        // Act
        var act = () => _sut.ExecuteAsync(serviceId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Failed to delete the service.");
    }
}
