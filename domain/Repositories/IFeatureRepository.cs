using domain.Entities;

namespace domain.Repositories;

public interface IFeatureRepository
{
    Task<Feature?> GetFeatureByIdAsync(Guid id);
    Task<Feature?> GetFeatureByNameAsync(string name);
    void AddFeature(Feature feature);
    void UpdateFeature(Feature feature);
    void DeleteFeature(Feature feature);
}
