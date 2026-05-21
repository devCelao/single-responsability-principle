using domain.Entities;
using FluentAssertions;
using infrastructure.Context;
using infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using test_srp.Helpers;

namespace test_srp.Integration.Repositories;

public class FeatureRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly FeatureRepository _sut;

    public FeatureRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _sut = new FeatureRepository(_context);
    }

    [Fact]
    public async Task AddFeature_ShouldPersistFeature()
    {
        // Arrange
        var feature = TestDataBuilder.CreateFeature();

        // Act
        _sut.AddFeature(feature);
        await _context.SaveChangesAsync();

        // Assert
        var savedFeature = await _context.Features.FindAsync(feature.Id);
        savedFeature.Should().NotBeNull();
        savedFeature!.Name.Should().Be(feature.Name);
    }

    [Fact]
    public async Task GetFeatureByIdAsync_ExistingId_ReturnsFeature()
    {
        // Arrange
        var feature = TestDataBuilder.CreateFeature(name: "Dashboard");
        _context.Features.Add(feature);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetFeatureByIdAsync(feature.Id);

        // Assert
        result.Should().NotBeNull();
        result?.Id.Should().Be(feature.Id);
        result?.Name.Should().Be("Dashboard");
    }

    [Fact]
    public async Task GetFeatureByIdAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = await _sut.GetFeatureByIdAsync(nonExistingId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetFeatureByNameAsync_ExistingName_ReturnsFeature()
    {
        // Arrange
        var feature = TestDataBuilder.CreateFeature(name: "Reports", permission: "read:reports");
        _context.Features.Add(feature);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetFeatureByNameAsync("Reports");

        // Assert
        result.Should().NotBeNull();
        result?.Name.Should().Be("Reports");
        result?.Permission.Should().Be("read:reports");
    }

    [Fact]
    public async Task GetFeatureByNameAsync_NonExistingName_ReturnsNull()
    {
        // Arrange
        var feature = TestDataBuilder.CreateFeature(name: "Dashboard");
        _context.Features.Add(feature);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetFeatureByNameAsync("NonExistent");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateFeature_ShouldModifyFeature()
    {
        // Arrange
        var feature = TestDataBuilder.CreateFeature(name: "Original Name");
        _context.Features.Add(feature);
        await _context.SaveChangesAsync();
        _context.Entry(feature).State = EntityState.Detached;

        // Act
        var featureToUpdate = await _context.Features.FindAsync(feature.Id);
        featureToUpdate!.UpdateFeature("Updated Name", "updated:permission");
        _sut.UpdateFeature(featureToUpdate);
        await _context.SaveChangesAsync();

        // Assert
        var updatedFeature = await _context.Features.FindAsync(feature.Id);
        updatedFeature!.Name.Should().Be("Updated Name");
        updatedFeature.Permission.Should().Be("updated:permission");
    }

    [Fact]
    public async Task DeleteFeature_ShouldRemoveFeature()
    {
        // Arrange
        var feature = TestDataBuilder.CreateFeature();
        _context.Features.Add(feature);
        await _context.SaveChangesAsync();

        // Act
        _sut.DeleteFeature(feature);
        await _context.SaveChangesAsync();

        // Assert
        var deletedFeature = await _context.Features.FindAsync(feature.Id);
        deletedFeature.Should().BeNull();
    }

    [Fact]
    public async Task AddFeature_WithAllProperties_PersistsCorrectly()
    {
        // Arrange
        var feature = TestDataBuilder.CreateFeature(
            name: "Admin Panel",
            permission: "admin:all");

        // Act
        _sut.AddFeature(feature);
        await _context.SaveChangesAsync();

        // Assert
        var savedFeature = await _context.Features.FindAsync(feature.Id);
        savedFeature.Should().NotBeNull();
        savedFeature!.Name.Should().Be("Admin Panel");
        savedFeature.Permission.Should().Be("admin:all");
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
