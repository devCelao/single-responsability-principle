using application.Dtos;
using application.UseCases.Plan;
using domain.Entities;
using domain.Repositories;
using FluentAssertions;
using Moq;
using test_srp.Helpers;

namespace test_srp.Unit.UseCases.Plan;

public class CreatePlanUseCaseTests
{
    private readonly Mock<IPlanRepository> _planRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreatePlanUseCase _sut;

    public CreatePlanUseCaseTests()
    {
        _planRepositoryMock = new Mock<IPlanRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new CreatePlanUseCase(_planRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidPlan_ReturnsGuid()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidPlanDto();
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        var result = await _sut.ExecuteAsync(dto);

        // Assert
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_ValidPlan_CallsAddPlan()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidPlanDto();
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        await _sut.ExecuteAsync(dto);

        // Assert
        _planRepositoryMock.Verify(r => r.AddPlan(It.IsAny<domain.Entities.Plan>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ValidPlan_CallsCommitAsync()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidPlanDto();
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        await _sut.ExecuteAsync(dto);

        // Assert
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithIsActiveTrue_CreatesActivePlan()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidPlanDto(isActive: true);
        domain.Entities.Plan? capturedPlan = null;
        
        _planRepositoryMock
            .Setup(r => r.AddPlan(It.IsAny<domain.Entities.Plan>()))
            .Callback<domain.Entities.Plan>(p => capturedPlan = p);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        await _sut.ExecuteAsync(dto);

        // Assert
        capturedPlan.Should().NotBeNull();
        capturedPlan!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_WithIsActiveFalse_CreatesInactivePlan()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidPlanDto(isActive: false);
        domain.Entities.Plan? capturedPlan = null;
        
        _planRepositoryMock
            .Setup(r => r.AddPlan(It.IsAny<domain.Entities.Plan>()))
            .Callback<domain.Entities.Plan>(p => capturedPlan = p);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        await _sut.ExecuteAsync(dto);

        // Assert
        capturedPlan.Should().NotBeNull();
        capturedPlan!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_CommitFails_ThrowsException()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidPlanDto();
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(false);

        // Act
        var act = () => _sut.ExecuteAsync(dto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Failed to create the plan.");
    }

    [Fact]
    public async Task ExecuteAsync_ValidPlan_CreatesPlanWithCorrectProperties()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidPlanDto(
            name: "Premium Plan",
            description: "Premium features",
            price: 299.99m);
        domain.Entities.Plan? capturedPlan = null;
        
        _planRepositoryMock
            .Setup(r => r.AddPlan(It.IsAny<domain.Entities.Plan>()))
            .Callback<domain.Entities.Plan>(p => capturedPlan = p);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(true);

        // Act
        await _sut.ExecuteAsync(dto);

        // Assert
        capturedPlan.Should().NotBeNull();
        capturedPlan!.Name.Should().Be("Premium Plan");
        capturedPlan.Description.Should().Be("Premium features");
        capturedPlan.Price.Should().Be(299.99m);
    }
}
