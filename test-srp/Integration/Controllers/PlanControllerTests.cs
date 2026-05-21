using System.Net;
using System.Net.Http.Json;
using application.Dtos;
using FluentAssertions;
using infrastructure.Context;
using Microsoft.Extensions.DependencyInjection;
using test_srp.Helpers;

namespace test_srp.Integration.Controllers;

public class PlanControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public PlanControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_ValidPlan_Returns201Created()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidPlanDto();

        // Act
        var response = await _client.PostAsJsonAsync("/api/plan", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_InvalidPlan_Returns400BadRequest()
    {
        // Arrange
        var dto = TestDataBuilder.CreateInvalidPlanDto();

        // Act
        var response = await _client.PostAsJsonAsync("/api/plan", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_EmptyName_Returns400BadRequest()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidPlanDto(name: "");

        // Act
        var response = await _client.PostAsJsonAsync("/api/plan", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_NegativePrice_Returns400BadRequest()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidPlanDto(price: -10);

        // Act
        var response = await _client.PostAsJsonAsync("/api/plan", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_ExistingPlan_Returns200WithPlan()
    {
        // Arrange
        var createDto = TestDataBuilder.CreateValidPlanDto(name: "Test Plan GetById");
        var createResponse = await _client.PostAsJsonAsync("/api/plan", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateResult>();

        // Act
        var response = await _client.GetAsync($"/api/plan/{createResult!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var plan = await response.Content.ReadFromJsonAsync<PlanDto>();
        plan.Should().NotBeNull();
        plan!.Name.Should().Be("Test Plan GetById");
    }

    [Fact]
    public async Task GetById_NonExistingPlan_ReturnsErrorResponse()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        Func<Task> act = async () => await _client.GetAsync($"/api/plan/{nonExistingId}");

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task Update_ValidPlan_Returns200Ok()
    {
        // Arrange
        var createDto = TestDataBuilder.CreateValidPlanDto(name: "Original Name");
        var createResponse = await _client.PostAsJsonAsync("/api/plan", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateResult>();

        var updateDto = TestDataBuilder.CreateValidPlanDto(name: "Updated Name");

        // Act
        var response = await _client.PutAsJsonAsync($"/api/plan/{createResult!.Id}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Update_InvalidPlan_Returns400BadRequest()
    {
        // Arrange
        var createDto = TestDataBuilder.CreateValidPlanDto();
        var createResponse = await _client.PostAsJsonAsync("/api/plan", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateResult>();

        var updateDto = TestDataBuilder.CreateInvalidPlanDto();

        // Act
        var response = await _client.PutAsJsonAsync($"/api/plan/{createResult!.Id}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_ExistingPlan_Returns200Ok()
    {
        // Arrange
        var createDto = TestDataBuilder.CreateValidPlanDto(name: "Plan to Delete");
        var createResponse = await _client.PostAsJsonAsync("/api/plan", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateResult>();

        // Act
        var response = await _client.DeleteAsync($"/api/plan/{createResult!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAll_WithPlans_ReturnsAllPlans()
    {
        // Arrange
        await _client.PostAsJsonAsync("/api/plan", TestDataBuilder.CreateValidPlanDto(name: "Plan A"));
        await _client.PostAsJsonAsync("/api/plan", TestDataBuilder.CreateValidPlanDto(name: "Plan B"));

        // Act
        var response = await _client.GetAsync("/api/plan");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var plans = await response.Content.ReadFromJsonAsync<List<PlanDto>>();
        plans.Should().NotBeNull();
        plans!.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    public void Dispose()
    {
        _client.Dispose();
        GC.SuppressFinalize(this);
    }

    private class CreateResult
    {
        public Guid Id { get; set; }
        public string? Message { get; set; }
    }
}
