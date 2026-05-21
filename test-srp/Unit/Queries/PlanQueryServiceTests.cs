using application.Queries;
using domain.Repositories;
using FluentAssertions;
using Moq;
using test_srp.Helpers;

namespace test_srp.Unit.Queries;

public class PlanQueryServiceTests
{
    private readonly Mock<IPlanRepository> _planRepositoryMock;
    private readonly PlanQueryService _sut;

    public PlanQueryServiceTests()
    {
        _planRepositoryMock = new Mock<IPlanRepository>();
        _sut = new PlanQueryService(_planRepositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingPlan_ReturnsDto()
    {
        // Arrange
        var plan = TestDataBuilder.CreatePlan(
            name: "Premium Plan",
            description: "Premium features",
            price: 199.99m);
        var planId = plan.Id;

        _planRepositoryMock
            .Setup(r => r.GetPlanByIdAsync(planId))
            .ReturnsAsync(plan);

        // Act
        var result = await _sut.GetByIdAsync(planId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(planId);
        result.Name.Should().Be("Premium Plan");
        result.Description.Should().Be("Premium features");
        result.Price.Should().Be(199.99m);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingPlan_MapsAllProperties()
    {
        // Arrange
        var plan = TestDataBuilder.CreatePlan();
        plan.Activate();
        var planId = plan.Id;

        _planRepositoryMock
            .Setup(r => r.GetPlanByIdAsync(planId))
            .ReturnsAsync(plan);

        // Act
        var result = await _sut.GetByIdAsync(planId);

        // Assert
        result.Should().NotBeNull();
        result!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetAllAsync_WithPlans_ReturnsAllPlans()
    {
        // Arrange
        var plans = new List<domain.Entities.Plan>
        {
            TestDataBuilder.CreatePlan(name: "Plan 1"),
            TestDataBuilder.CreatePlan(name: "Plan 2"),
            TestDataBuilder.CreatePlan(name: "Plan 3")
        };

        _planRepositoryMock
            .Setup(r => r.GetAllPlansAsync())
            .ReturnsAsync(plans);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Select(p => p.Name).Should().Contain(new[] { "Plan 1", "Plan 2", "Plan 3" });
    }

    [Fact]
    public async Task GetAllAsync_WithNoPlans_ReturnsEmptyCollection()
    {
        // Arrange
        _planRepositoryMock
            .Setup(r => r.GetAllPlansAsync())
            .ReturnsAsync(new List<domain.Entities.Plan>());

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_CallsRepository()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var plan = TestDataBuilder.CreatePlan();

        _planRepositoryMock
            .Setup(r => r.GetPlanByIdAsync(planId))
            .ReturnsAsync(plan);

        // Act
        await _sut.GetByIdAsync(planId);

        // Assert
        _planRepositoryMock.Verify(r => r.GetPlanByIdAsync(planId), Times.Once);
    }
}
