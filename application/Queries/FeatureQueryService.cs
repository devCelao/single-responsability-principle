using application.Dtos;
using domain.Entities;
using domain.Repositories;

namespace application.Queries;

public class FeatureQueryService(IFeatureRepository featureRepository)
{
    private readonly IFeatureRepository _featureRepository = featureRepository;
    public async Task<FeatureDto?> GetByIdAsync(Guid id)
    {
        var feature = await _featureRepository.GetFeatureByIdAsync(id) ?? throw new KeyNotFoundException($"Feature with id {id} not found.");
        return MapToDto(feature);
    }
    public async Task<FeatureDto?> GetFeatureByNameAsync(string name)
    {
        var feature = await _featureRepository.GetFeatureByNameAsync(name);
        return feature == null ? null : MapToDto(feature);
    }
    private static FeatureDto MapToDto(Feature feature) => new()
    {
        Id = feature.Id,
        Name = feature.Name,
        Permission = feature.Permission
    };
}
