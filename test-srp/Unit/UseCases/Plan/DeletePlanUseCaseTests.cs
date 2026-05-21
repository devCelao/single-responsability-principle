using application.UseCases.Plan;
using domain.Entities;
using domain.Repositories;
using FluentAssertions;
using Moq;
using test_srp.Helpers;

namespace test_srp.Unit.UseCases.Plan;

public class DeletePlanUseCaseTests
{
    private readonly Mock<IPlanRepository> _planRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DeletePlanUseCase _sut;

    public DeletePlanUseCaseTests()
    {
        _planRepositoryMock = new Mock<IPlanRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new DeletePlanUseCase(_planRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ExistingPlan_DeletesSuccessfully()
    {
        // Arrange
        var existingPlan = TestDataBuilder.CreatePlan();
        var planId = existingPlan.Id;

        _planRepositoryMock
            .Setup(r => r.GetPlanByIdAsync(planId))
            .ReturnsAsync(existingPlan);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        var act = () => _sut.ExecuteAsync(planId);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ExecuteAsync_ExistingPlan_CallsDeletePlanAsync()
    {
        // Arrange
        var existingPlan = TestDataBuilder.CreatePlan();
        var planId = existingPlan.Id;

        _planRepositoryMock
            .Setup(r => r.GetPlanByIdAsync(planId))
            .ReturnsAsync(existingPlan);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        await _sut.ExecuteAsync(planId);

        // Assert
        _planRepositoryMock.Verify(r => r.DeletePlanAsync(existingPlan), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ExistingPlan_CallsCommitAsync()
    {
        // Arrange
        var existingPlan = TestDataBuilder.CreatePlan();
        var planId = existingPlan.Id;

        _planRepositoryMock
            .Setup(r => r.GetPlanByIdAsync(planId))
            .ReturnsAsync(existingPlan);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        await _sut.ExecuteAsync(planId);

        // Assert
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_PlanNotFound_ThrowsException()
    {
        // Arrange
        var planId = Guid.NewGuid();

        _planRepositoryMock
            .Setup(r => r.GetPlanByIdAsync(planId))
            .ReturnsAsync((domain.Entities.Plan?)null);

        // Act
        var act = () => _sut.ExecuteAsync(planId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Plan not found");
    }

    [Fact]
    public async Task ExecuteAsync_CommitFails_ThrowsException()
    {
        // Arrange
        var existingPlan = TestDataBuilder.CreatePlan();
        var planId = existingPlan.Id;

        _planRepositoryMock
            .Setup(r => r.GetPlanByIdAsync(planId))
            .ReturnsAsync(existingPlan);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(false);

        // Act
        var act = () => _sut.ExecuteAsync(planId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Failed to delete the plan.");
    }
}
