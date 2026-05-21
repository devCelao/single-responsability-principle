using domain.Entities;
using FluentAssertions;
using infrastructure.Context;
using infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using test_srp.Helpers;

namespace test_srp.Integration.Repositories;

public class PlanRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly PlanRepository _sut;

    public PlanRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _sut = new PlanRepository(_context);
    }

    [Fact]
    public async Task AddPlan_ShouldPersistPlan()
    {
        // Arrange
        var plan = TestDataBuilder.CreatePlan();

        // Act
        _sut.AddPlan(plan);
        await _context.SaveChangesAsync();

        // Assert
        var savedPlan = await _context.Plans.FindAsync(plan.Id);
        savedPlan.Should().NotBeNull();
        savedPlan!.Name.Should().Be(plan.Name);
    }

    [Fact]
    public async Task GetPlanByIdAsync_ExistingId_ReturnsPlan()
    {
        // Arrange
        var plan = TestDataBuilder.CreatePlan(name: "Test Plan");
        _context.Plans.Add(plan);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetPlanByIdAsync(plan.Id);

        // Assert
        result.Should().NotBeNull();
        result?.Id.Should().Be(plan.Id);
        result?.Name.Should().Be("Test Plan");
    }

    [Fact]
    public async Task GetPlanByIdAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = await _sut.GetPlanByIdAsync(nonExistingId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllPlansAsync_WithPlans_ReturnsAllPlans()
    {
        // Arrange
        var plans = new List<Plan>
        {
            TestDataBuilder.CreatePlan(name: "Plan 1"),
            TestDataBuilder.CreatePlan(name: "Plan 2"),
            TestDataBuilder.CreatePlan(name: "Plan 3")
        };
        _context.Plans.AddRange(plans);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAllPlansAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllPlansAsync_WithNoPlans_ReturnsEmptyCollection()
    {
        // Act
        var result = await _sut.GetAllPlansAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdatePlan_ShouldModifyPlan()
    {
        // Arrange
        var plan = TestDataBuilder.CreatePlan(name: "Original Name");
        _context.Plans.Add(plan);
        await _context.SaveChangesAsync();
        _context.Entry(plan).State = EntityState.Detached;

        // Act
        var planToUpdate = await _context.Plans.FindAsync(plan.Id);
        planToUpdate!.UpdatePlan("Updated Name", "Updated Description", 299.99m, true);
        _sut.UpdatePlan(planToUpdate);
        await _context.SaveChangesAsync();

        // Assert
        var updatedPlan = await _context.Plans.FindAsync(plan.Id);
        updatedPlan!.Name.Should().Be("Updated Name");
        updatedPlan.Description.Should().Be("Updated Description");
        updatedPlan.Price.Should().Be(299.99m);
        updatedPlan.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task DeletePlanAsync_ShouldRemovePlan()
    {
        // Arrange
        var plan = TestDataBuilder.CreatePlan();
        _context.Plans.Add(plan);
        await _context.SaveChangesAsync();

        // Act
        _sut.DeletePlanAsync(plan);
        await _context.SaveChangesAsync();

        // Assert
        var deletedPlan = await _context.Plans.FindAsync(plan.Id);
        deletedPlan.Should().BeNull();
    }

    [Fact]
    public async Task AddPlan_WithAllProperties_PersistsCorrectly()
    {
        // Arrange
        var plan = TestDataBuilder.CreatePlan(
            name: "Premium Plan",
            description: "Premium features",
            price: 199.99m);
        plan.Activate();

        // Act
        _sut.AddPlan(plan);
        await _context.SaveChangesAsync();

        // Assert
        var savedPlan = await _context.Plans.FindAsync(plan.Id);
        savedPlan.Should().NotBeNull();
        savedPlan!.Name.Should().Be("Premium Plan");
        savedPlan.Description.Should().Be("Premium features");
        savedPlan.Price.Should().Be(199.99m);
        savedPlan.IsActive.Should().BeTrue();
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
