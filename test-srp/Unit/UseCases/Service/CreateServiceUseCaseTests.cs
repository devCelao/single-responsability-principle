using application.Dtos;
using application.UseCases.Service;
using domain.Repositories;
using FluentAssertions;
using Moq;
using test_srp.Helpers;

namespace test_srp.Unit.UseCases.Service;

public class CreateServiceUseCaseTests
{
    private readonly Mock<IServiceRepository> _serviceRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateServiceUseCase _sut;

    public CreateServiceUseCaseTests()
    {
        _serviceRepositoryMock = new Mock<IServiceRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new CreateServiceUseCase(_serviceRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidService_ReturnsGuid()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidServiceDto();
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        var result = await _sut.ExecuteAsync(dto);

        // Assert
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_ValidService_CallsAddService()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidServiceDto();
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        await _sut.ExecuteAsync(dto);

        // Assert
        _serviceRepositoryMock.Verify(r => r.AddService(It.IsAny<domain.Entities.Service>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ValidService_CallsCommitAsync()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidServiceDto();
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        await _sut.ExecuteAsync(dto);

        // Assert
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ValidService_CreatesServiceWithCorrectProperties()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidServiceDto(
            name: "API Service",
            type: "Infrastructure");
        domain.Entities.Service? capturedService = null;
        
        _serviceRepositoryMock
            .Setup(r => r.AddService(It.IsAny<domain.Entities.Service>()))
            .Callback<domain.Entities.Service>(s => capturedService = s);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        await _sut.ExecuteAsync(dto);

        // Assert
        capturedService.Should().NotBeNull();
        capturedService!.Name.Should().Be("API Service");
        capturedService.Type.Should().Be("Infrastructure");
    }

    [Fact]
    public async Task ExecuteAsync_CommitFails_ThrowsException()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidServiceDto();
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(false);

        // Act
        var act = () => _sut.ExecuteAsync(dto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Failed to create the service.");
    }
}
