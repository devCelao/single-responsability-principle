using application.UseCases.Feature;
using domain.Repositories;
using FluentAssertions;
using Moq;
using test_srp.Helpers;

namespace test_srp.Unit.UseCases.Feature;

public class DeleteFeatureUseCaseTests
{
    private readonly Mock<IFeatureRepository> _featureRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DeleteFeatureUseCase _sut;

    public DeleteFeatureUseCaseTests()
    {
        _featureRepositoryMock = new Mock<IFeatureRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new DeleteFeatureUseCase(_featureRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ExistingFeature_DeletesSuccessfully()
    {
        // Arrange
        var existingFeature = TestDataBuilder.CreateFeature();
        var featureId = existingFeature.Id;

        _featureRepositoryMock
            .Setup(r => r.GetFeatureByIdAsync(featureId))
            .ReturnsAsync(existingFeature);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        var act = () => _sut.ExecuteAsync(featureId);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ExecuteAsync_ExistingFeature_CallsDeleteFeature()
    {
        // Arrange
        var existingFeature = TestDataBuilder.CreateFeature();
        var featureId = existingFeature.Id;

        _featureRepositoryMock
            .Setup(r => r.GetFeatureByIdAsync(featureId))
            .ReturnsAsync(existingFeature);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        await _sut.ExecuteAsync(featureId);

        // Assert
        _featureRepositoryMock.Verify(r => r.DeleteFeature(existingFeature), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ExistingFeature_CallsCommitAsync()
    {
        // Arrange
        var existingFeature = TestDataBuilder.CreateFeature();
        var featureId = existingFeature.Id;

        _featureRepositoryMock
            .Setup(r => r.GetFeatureByIdAsync(featureId))
            .ReturnsAsync(existingFeature);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        await _sut.ExecuteAsync(featureId);

        // Assert
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_FeatureNotFound_ThrowsException()
    {
        // Arrange
        var featureId = Guid.NewGuid();

        _featureRepositoryMock
            .Setup(r => r.GetFeatureByIdAsync(featureId))
            .ReturnsAsync((domain.Entities.Feature?)null);

        // Act
        var act = () => _sut.ExecuteAsync(featureId);

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

        _featureRepositoryMock
            .Setup(r => r.GetFeatureByIdAsync(featureId))
            .ReturnsAsync(existingFeature);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(false);

        // Act
        var act = () => _sut.ExecuteAsync(featureId);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*error occurred while deleting*");
    }
}
