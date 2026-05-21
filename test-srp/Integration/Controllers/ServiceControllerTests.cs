using System.Net;
using System.Net.Http.Json;
using application.Dtos;
using FluentAssertions;
using test_srp.Helpers;

namespace test_srp.Integration.Controllers;

public class ServiceControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public ServiceControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_ValidService_Returns201Created()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidServiceDto();

        // Act
        var response = await _client.PostAsJsonAsync("/api/service", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_InvalidService_Returns400BadRequest()
    {
        // Arrange
        var dto = TestDataBuilder.CreateInvalidServiceDto();

        // Act
        var response = await _client.PostAsJsonAsync("/api/service", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_EmptyName_Returns400BadRequest()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidServiceDto(name: "");

        // Act
        var response = await _client.PostAsJsonAsync("/api/service", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_EmptyType_Returns400BadRequest()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidServiceDto(type: "");

        // Act
        var response = await _client.PostAsJsonAsync("/api/service", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_ExistingService_Returns200WithService()
    {
        // Arrange
        var createDto = TestDataBuilder.CreateValidServiceDto(name: "Test Service GetById");
        var createResponse = await _client.PostAsJsonAsync("/api/service", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateResult>();

        // Act
        var response = await _client.GetAsync($"/api/service/{createResult!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var service = await response.Content.ReadFromJsonAsync<ServiceDto>();
        service.Should().NotBeNull();
        service!.Name.Should().Be("Test Service GetById");
    }

    [Fact]
    public async Task GetById_NonExistingService_ReturnsNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/service/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ValidService_Returns200Ok()
    {
        // Arrange
        var createDto = TestDataBuilder.CreateValidServiceDto(name: "Original Service");
        var createResponse = await _client.PostAsJsonAsync("/api/service", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateResult>();

        var updateDto = TestDataBuilder.CreateValidServiceDto(name: "Updated Service");

        // Act
        var response = await _client.PutAsJsonAsync($"/api/service/{createResult!.Id}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Update_InvalidService_Returns400BadRequest()
    {
        // Arrange
        var createDto = TestDataBuilder.CreateValidServiceDto();
        var createResponse = await _client.PostAsJsonAsync("/api/service", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateResult>();

        var updateDto = TestDataBuilder.CreateInvalidServiceDto();

        // Act
        var response = await _client.PutAsJsonAsync($"/api/service/{createResult!.Id}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_ExistingService_Returns200Ok()
    {
        // Arrange
        var createDto = TestDataBuilder.CreateValidServiceDto(name: "Service to Delete");
        var createResponse = await _client.PostAsJsonAsync("/api/service", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateResult>();

        // Act
        var response = await _client.DeleteAsync($"/api/service/{createResult!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetByType_WithMatchingServices_ReturnsFilteredServices()
    {
        // Arrange
        await _client.PostAsJsonAsync("/api/service", TestDataBuilder.CreateValidServiceDto(name: "Service 1", type: "Premium"));
        await _client.PostAsJsonAsync("/api/service", TestDataBuilder.CreateValidServiceDto(name: "Service 2", type: "Premium"));

        // Act
        var response = await _client.GetAsync("/api/service/type/Premium");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var services = await response.Content.ReadFromJsonAsync<List<ServiceDto>>();
        services.Should().NotBeNull();
        services!.Count.Should().BeGreaterThanOrEqualTo(2);
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
