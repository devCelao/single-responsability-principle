using application.Dtos;
using application.UseCases.Plan;
using domain.Entities;
using domain.Repositories;
using FluentAssertions;
using Moq;
using test_srp.Helpers;

namespace test_srp.Unit.UseCases.Plan;

public class UpdatePlanUseCaseTests
{
    private readonly Mock<IPlanRepository> _planRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdatePlanUseCase _sut;

    public UpdatePlanUseCaseTests()
    {
        _planRepositoryMock = new Mock<IPlanRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new UpdatePlanUseCase(_planRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ExistingPlan_UpdatesAndReturnsId()
    {
        // Arrange
        var existingPlan = TestDataBuilder.CreatePlan();
        var planId = existingPlan.Id;
        var updateDto = TestDataBuilder.CreateValidPlanDto(
            name: "Updated Name",
            description: "Updated Description",
            price: 199.99m);

        _planRepositoryMock
            .Setup(r => r.GetPlanByIdAsync(planId))
            .ReturnsAsync(existingPlan);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        var result = await _sut.ExecuteAsync(planId, updateDto);

        // Assert
        result.Should().Be(planId);
    }

    [Fact]
    public async Task ExecuteAsync_ExistingPlan_CallsUpdatePlan()
    {
        // Arrange
        var existingPlan = TestDataBuilder.CreatePlan();
        var planId = existingPlan.Id;
        var updateDto = TestDataBuilder.CreateValidPlanDto();

        _planRepositoryMock
            .Setup(r => r.GetPlanByIdAsync(planId))
            .ReturnsAsync(existingPlan);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        await _sut.ExecuteAsync(planId, updateDto);

        // Assert
        _planRepositoryMock.Verify(r => r.UpdatePlan(It.IsAny<domain.Entities.Plan>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ExistingPlan_UpdatesPlanProperties()
    {
        // Arrange
        var existingPlan = TestDataBuilder.CreatePlan();
        var planId = existingPlan.Id;
        var updateDto = TestDataBuilder.CreateValidPlanDto(
            name: "New Name",
            description: "New Description",
            price: 299.99m,
            isActive: true);

        _planRepositoryMock
            .Setup(r => r.GetPlanByIdAsync(planId))
            .ReturnsAsync(existingPlan);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        await _sut.ExecuteAsync(planId, updateDto);

        // Assert
        existingPlan.Name.Should().Be("New Name");
        existingPlan.Description.Should().Be("New Description");
        existingPlan.Price.Should().Be(299.99m);
        existingPlan.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_PlanNotFound_ThrowsException()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var updateDto = TestDataBuilder.CreateValidPlanDto();

        _planRepositoryMock
            .Setup(r => r.GetPlanByIdAsync(planId))
            .ReturnsAsync((domain.Entities.Plan?)null);

        // Act
        var act = () => _sut.ExecuteAsync(planId, updateDto);

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
        var updateDto = TestDataBuilder.CreateValidPlanDto();

        _planRepositoryMock
            .Setup(r => r.GetPlanByIdAsync(planId))
            .ReturnsAsync(existingPlan);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(false);

        // Act
        var act = () => _sut.ExecuteAsync(planId, updateDto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Failed to update the plan.");
    }
}
