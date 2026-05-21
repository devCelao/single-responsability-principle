using application.Dtos;
using application.UseCases.Feature;
using domain.Repositories;
using FluentAssertions;
using Moq;
using test_srp.Helpers;

namespace test_srp.Unit.UseCases.Feature;

public class CreateFeatureUseCaseTests
{
    private readonly Mock<IFeatureRepository> _featureRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateFeatureUseCase _sut;

    public CreateFeatureUseCaseTests()
    {
        _featureRepositoryMock = new Mock<IFeatureRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new CreateFeatureUseCase(_featureRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidFeature_ReturnsGuid()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidFeatureDto();
        _featureRepositoryMock
            .Setup(r => r.GetFeatureByNameAsync(dto.Name))
            .ReturnsAsync((domain.Entities.Feature?)null);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        var result = await _sut.ExecuteAsync(dto);

        // Assert
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_ValidFeature_CallsAddFeature()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidFeatureDto();
        _featureRepositoryMock
            .Setup(r => r.GetFeatureByNameAsync(dto.Name))
            .ReturnsAsync((domain.Entities.Feature?)null);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        await _sut.ExecuteAsync(dto);

        // Assert
        _featureRepositoryMock.Verify(r => r.AddFeature(It.IsAny<domain.Entities.Feature>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ValidFeature_CallsCommitAsync()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidFeatureDto();
        _featureRepositoryMock
            .Setup(r => r.GetFeatureByNameAsync(dto.Name))
            .ReturnsAsync((domain.Entities.Feature?)null);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        await _sut.ExecuteAsync(dto);

        // Assert
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ValidFeature_CreatesFeatureWithCorrectProperties()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidFeatureDto(
            name: "Dashboard",
            permission: "read:dashboard");
        domain.Entities.Feature? capturedFeature = null;
        
        _featureRepositoryMock
            .Setup(r => r.GetFeatureByNameAsync(dto.Name))
            .ReturnsAsync((domain.Entities.Feature?)null);
        _featureRepositoryMock
            .Setup(r => r.AddFeature(It.IsAny<domain.Entities.Feature>()))
            .Callback<domain.Entities.Feature>(f => capturedFeature = f);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        await _sut.ExecuteAsync(dto);

        // Assert
        capturedFeature.Should().NotBeNull();
        capturedFeature!.Name.Should().Be("Dashboard");
        capturedFeature.Permission.Should().Be("read:dashboard");
    }

    [Fact]
    public async Task ExecuteAsync_DuplicateName_ThrowsException()
    {
        // Arrange
        var existingFeature = TestDataBuilder.CreateFeature(name: "ExistingFeature");
        var dto = TestDataBuilder.CreateValidFeatureDto(name: "ExistingFeature");
        
        _featureRepositoryMock
            .Setup(r => r.GetFeatureByNameAsync(dto.Name))
            .ReturnsAsync(existingFeature);

        // Act
        var act = () => _sut.ExecuteAsync(dto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*ExistingFeature*already exists*");
    }

    [Fact]
    public async Task ExecuteAsync_CommitFails_ThrowsException()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidFeatureDto();
        _featureRepositoryMock
            .Setup(r => r.GetFeatureByNameAsync(dto.Name))
            .ReturnsAsync((domain.Entities.Feature?)null);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(false);

        // Act
        var act = () => _sut.ExecuteAsync(dto);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*error occurred while saving*");
    }
}
