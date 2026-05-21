using System.Net;
using System.Net.Http.Json;
using application.Dtos;
using FluentAssertions;
using test_srp.Helpers;

namespace test_srp.Integration.Controllers;

public class FeatureControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public FeatureControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_ValidFeature_Returns201Created()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidFeatureDto(name: $"Feature_{Guid.NewGuid()}");

        // Act
        var response = await _client.PostAsJsonAsync("/api/feature", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_InvalidFeature_Returns400BadRequest()
    {
        // Arrange
        var dto = TestDataBuilder.CreateInvalidFeatureDto();

        // Act
        var response = await _client.PostAsJsonAsync("/api/feature", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_EmptyName_Returns400BadRequest()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidFeatureDto(name: "");

        // Act
        var response = await _client.PostAsJsonAsync("/api/feature", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_EmptyPermission_Returns400BadRequest()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidFeatureDto(permission: "");

        // Act
        var response = await _client.PostAsJsonAsync("/api/feature", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_ExistingFeature_Returns200WithFeature()
    {
        // Arrange
        var featureName = $"Test Feature GetById_{Guid.NewGuid()}";
        var createDto = TestDataBuilder.CreateValidFeatureDto(name: featureName);
        var createResponse = await _client.PostAsJsonAsync("/api/feature", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateResult>();

        // Act
        var response = await _client.GetAsync($"/api/feature/{createResult!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var feature = await response.Content.ReadFromJsonAsync<FeatureDto>();
        feature.Should().NotBeNull();
        feature!.Name.Should().Be(featureName);
    }

    [Fact]
    public async Task GetById_NonExistingFeature_ReturnsErrorResponse()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/feature/{nonExistingId}");

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Update_ValidFeature_Returns200Ok()
    {
        // Arrange
        var originalName = $"Original Feature_{Guid.NewGuid()}";
        var createDto = TestDataBuilder.CreateValidFeatureDto(name: originalName);
        var createResponse = await _client.PostAsJsonAsync("/api/feature", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateResult>();

        var updateDto = TestDataBuilder.CreateValidFeatureDto(name: $"Updated Feature_{Guid.NewGuid()}");

        // Act
        var response = await _client.PutAsJsonAsync($"/api/feature/{createResult!.Id}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Update_InvalidFeature_Returns400BadRequest()
    {
        // Arrange
        var featureName = $"Feature to Update_{Guid.NewGuid()}";
        var createDto = TestDataBuilder.CreateValidFeatureDto(name: featureName);
        var createResponse = await _client.PostAsJsonAsync("/api/feature", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateResult>();

        var updateDto = TestDataBuilder.CreateInvalidFeatureDto();

        // Act
        var response = await _client.PutAsJsonAsync($"/api/feature/{createResult!.Id}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_ExistingFeature_Returns200Ok()
    {
        // Arrange
        var featureName = $"Feature to Delete_{Guid.NewGuid()}";
        var createDto = TestDataBuilder.CreateValidFeatureDto(name: featureName);
        var createResponse = await _client.PostAsJsonAsync("/api/feature", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateResult>();

        // Act
        var response = await _client.DeleteAsync($"/api/feature/{createResult!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
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
