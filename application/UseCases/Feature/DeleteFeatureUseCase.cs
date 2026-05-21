using domain.Repositories;

namespace application.UseCases.Feature;

public class DeleteFeatureUseCase(IFeatureRepository featureRepository, IUnitOfWork unitOfWork)
{
    private readonly IFeatureRepository _featureRepository = featureRepository;
    private readonly IUnitOfWork _uow = unitOfWork;
    public async Task ExecuteAsync(Guid idFeature)
    {
        var feature = await _featureRepository.GetFeatureByIdAsync(idFeature) ?? throw new InvalidOperationException("Feature not found");
        _featureRepository.DeleteFeature(feature);
        if(! await _uow.CommitAsync())
            throw new Exception("An error occurred while deleting the feature.");
    }
}