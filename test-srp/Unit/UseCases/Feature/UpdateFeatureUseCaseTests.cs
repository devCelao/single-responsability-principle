using application.Dtos;
using application.UseCases.Feature;
using domain.Repositories;
using FluentAssertions;
using Moq;
using test_srp.Helpers;

namespace test_srp.Unit.UseCases.Feature;

public class UpdateFeatureUseCaseTests
{
    private readonly Mock<IFeatureRepository> _featureRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateFeatureUseCase _sut;

    public UpdateFeatureUseCaseTests()
    {
        _featureRepositoryMock = new Mock<IFeatureRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new UpdateFeatureUseCase(_featureRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ExistingFeature_UpdatesAndReturnsId()
    {
        // Arrange
        var existingFeature = TestDataBuilder.CreateFeature();
        var featureId = existingFeature.Id;
        var updateDto = TestDataBuilder.CreateValidFeatureDto(
            name: "Updated Name",
            permission: "updated:permission");

        _featureRepositoryMock
            .Setup(r => r.GetFeatureByIdAsync(featureId))
            .ReturnsAsync(existingFeature);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        var result = await _sut.ExecuteAsync(featureId, updateDto);

        // Assert
        result.Should().Be(featureId);
    }

    [Fact]
    public async Task ExecuteAsync_ExistingFeature_CallsUpdateFeature()
    {
        // Arrange
        var existingFeature = TestDataBuilder.CreateFeature();
        var featureId = existingFeature.Id;
        var updateDto = TestDataBuilder.CreateValidFeatureDto();

        _featureRepositoryMock
            .Setup(r => r.GetFeatureByIdAsync(featureId))
            .ReturnsAsync(existingFeature);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        await _sut.ExecuteAsync(featureId, updateDto);

        // Assert
        _featureRepositoryMock.Verify(r => r.UpdateFeature(It.IsAny<domain.Entities.Feature>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ExistingFeature_UpdatesFeatureProperties()
    {
        // Arrange
        var existingFeature = TestDataBuilder.CreateFeature();
        var featureId = existingFeature.Id;
        var updateDto = TestDataBuilder.CreateValidFeatureDto(
            name: "New Name",
            permission: "new:permission");

        _featureRepositoryMock
            .Setup(r => r.GetFeatureByIdAsync(featureId))
            .ReturnsAsync(existingFeature);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        await _sut.ExecuteAsync(featureId, updateDto);

        // Assert
        existingFeature.Name.Should().Be("New Name");
        existingFeature.Permission.Should().Be("new:permission");
    }

    [Fact]
    public async Task ExecuteAsync_FeatureNotFound_ThrowsException()
    {
        // Arrange
        var featureId = Guid.NewGuid();
        var updateDto = TestDataBuilder.CreateValidFeatureDto();

        _featureRepositoryMock
            .Setup(r => r.GetFeatureByIdAsync(featureId))
            .ReturnsAsync((domain.Entities.Feature?)null);

        // Act
        var act = () => _sut.ExecuteAsync(featureId, updateDto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Feature not found");
    }

    [Fact]
    public async Task ExecuteAsync_CommitFails_ThrowsException()
    {
        // Arrange
        var existingFeature = TestDataBuilder.CreateFeature();
        var featureId = existingFeature.Id;
        var updateDto = TestDataBuilder.CreateValidFeatureDto();

        _featureRepositoryMock
            .Setup(r => r.GetFeatureByIdAsync(featureId))
            .ReturnsAsync(existingFeature);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(false);

        // Act
        var act = () => _sut.ExecuteAsync(featureId, updateDto);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*error occurred while updating*");
    }
}
