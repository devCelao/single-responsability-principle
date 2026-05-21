using domain.Repositories;

namespace application.UseCases;

public class BindFeatureToServiceUseCase(IFeatureRepository featureRepository, IServiceRepository serviceRepository, IUnitOfWork unitOfWork)
{
    private readonly IFeatureRepository _featureRepository = featureRepository;
    private readonly IServiceRepository _serviceRepository = serviceRepository;
    private readonly IUnitOfWork _uow = unitOfWork;
    public async Task ExecuteAsync(Guid featureId, Guid serviceId)
    {
        var feature = await _featureRepository.GetFeatureByIdAsync(featureId) ?? throw new KeyNotFoundException("Feature not found.");
        var service = await _serviceRepository.GetServiceByIdAsync(serviceId) ?? throw new KeyNotFoundException("Service not found.");
        if (service.Features.Any(f => f.Id == featureId))
            throw new InvalidOperationException("The feature is already linked to the service.");
        service.Features.Add(feature);
        _serviceRepository.UpdateService(service);
        if(!await _uow.CommitAsync())
            throw new Exception("Failed to bind feature to service.");
    }
}
