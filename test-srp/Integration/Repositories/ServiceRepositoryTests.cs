using domain.Entities;
using FluentAssertions;
using infrastructure.Context;
using infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using test_srp.Helpers;

namespace test_srp.Integration.Repositories;

public class ServiceRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ServiceRepository _sut;

    public ServiceRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _sut = new ServiceRepository(_context);
    }

    [Fact]
    public async Task AddService_ShouldPersistService()
    {
        // Arrange
        var service = TestDataBuilder.CreateService();

        // Act
        _sut.AddService(service);
        await _context.SaveChangesAsync();

        // Assert
        var savedService = await _context.Services.FindAsync(service.Id);
        savedService.Should().NotBeNull();
        savedService!.Name.Should().Be(service.Name);
    }

    [Fact]
    public async Task GetServiceByIdAsync_ExistingId_ReturnsService()
    {
        // Arrange
        var service = TestDataBuilder.CreateService(name: "Test Service");
        _context.Services.Add(service);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetServiceByIdAsync(service.Id);

        // Assert
        result.Should().NotBeNull();
        result?.Id.Should().Be(service.Id);
        result?.Name.Should().Be("Test Service");
    }

    [Fact]
    public async Task GetServiceByIdAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = await _sut.GetServiceByIdAsync(nonExistingId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByTypeAsync_WithMatchingServices_ReturnsFilteredServices()
    {
        // Arrange
        var services = new List<Service>
        {
            TestDataBuilder.CreateService(name: "Service 1", type: "Premium"),
            TestDataBuilder.CreateService(name: "Service 2", type: "Premium"),
            TestDataBuilder.CreateService(name: "Service 3", type: "Basic")
        };
        _context.Services.AddRange(services);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetByTypeAsync("Premium");

        // Assert
        result.Should().HaveCount(2);
        result.All(s => s.Type == "Premium").Should().BeTrue();
    }

    [Fact]
    public async Task GetByTypeAsync_WithNoMatchingServices_ReturnsEmptyCollection()
    {
        // Arrange
        var service = TestDataBuilder.CreateService(type: "Premium");
        _context.Services.Add(service);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetByTypeAsync("NonExistent");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateService_ShouldModifyService()
    {
        // Arrange
        var service = TestDataBuilder.CreateService(name: "Original Name");
        _context.Services.Add(service);
        await _context.SaveChangesAsync();
        _context.Entry(service).State = EntityState.Detached;

        // Act
        var serviceToUpdate = await _context.Services.FindAsync(service.Id);
        serviceToUpdate!.UpdateService("Updated Name", "Updated Type", true);
        _sut.UpdateService(serviceToUpdate);
        await _context.SaveChangesAsync();

        // Assert
        var updatedService = await _context.Services.FindAsync(service.Id);
        updatedService!.Name.Should().Be("Updated Name");
        updatedService.Type.Should().Be("Updated Type");
        updatedService.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteServiceAsync_ShouldRemoveService()
    {
        // Arrange
        var service = TestDataBuilder.CreateService();
        _context.Services.Add(service);
        await _context.SaveChangesAsync();

        // Act
        _sut.DeleteServiceAsync(service);
        await _context.SaveChangesAsync();

        // Assert
        var deletedService = await _context.Services.FindAsync(service.Id);
        deletedService.Should().BeNull();
    }

    [Fact]
    public async Task AddService_WithAllProperties_PersistsCorrectly()
    {
        // Arrange
        var service = TestDataBuilder.CreateService(
            name: "API Gateway",
            type: "Infrastructure");
        service.Activate();

        // Act
        _sut.AddService(service);
        await _context.SaveChangesAsync();

        // Assert
        var savedService = await _context.Services.FindAsync(service.Id);
        savedService.Should().NotBeNull();
        savedService!.Name.Should().Be("API Gateway");
        savedService.Type.Should().Be("Infrastructure");
        savedService.IsActive.Should().BeTrue();
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
