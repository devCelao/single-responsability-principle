using application.Dtos;
using domain.Repositories;

namespace application.UseCases.Feature;

public class UpdateFeatureUseCase(IFeatureRepository featureRepository, IUnitOfWork unitOfWork)
{
    private readonly IFeatureRepository _featureRepository = featureRepository;
    private readonly IUnitOfWork _uow = unitOfWork;
    public async Task<Guid> ExecuteAsync(Guid idFeature, CreateFeatureDto updateFeature)
    {
        var existingFeature = await _featureRepository.GetFeatureByIdAsync(idFeature) ?? throw new InvalidOperationException("Feature not found");
        existingFeature.UpdateFeature(updateFeature.Name, updateFeature.Permission);
        _featureRepository.UpdateFeature(existingFeature);
        if(! await _uow.CommitAsync())
            throw new Exception("An error occurred while updating the feature.");
        return existingFeature.Id;
    }
}
