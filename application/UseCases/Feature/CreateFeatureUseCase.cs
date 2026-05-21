using application.Dtos;
using domain.Repositories;

namespace application.UseCases.Feature;

public class CreateFeatureUseCase(IFeatureRepository featureRepository, IUnitOfWork unitOfWork)
{
    private readonly IFeatureRepository _featureRepository = featureRepository;

    private readonly IUnitOfWork _uow = unitOfWork;
    public async Task<Guid> ExecuteAsync(CreateFeatureDto createFeature)
    {
       var existingFeature = await _featureRepository.GetFeatureByNameAsync(createFeature.Name);

        if (existingFeature != null)
            throw new InvalidOperationException($"A feature with the name '{createFeature.Name}' already exists.");

        var feature = new domain.Entities.Feature(createFeature.Name, createFeature.Permission);

        _featureRepository.AddFeature(feature);

        if(! await _uow.CommitAsync())
            throw new Exception("An error occurred while saving the feature.");

        return feature.Id;
    }
}
