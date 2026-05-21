using application.Dtos;
using domain.Repositories;

namespace application.UseCases.Service;

public class UpdateServiceUseCase(IServiceRepository serviceRepository, IUnitOfWork unitOfWork)
{
    private readonly IServiceRepository _serviceRepository = serviceRepository;
    private readonly IUnitOfWork _uow = unitOfWork;
    public async Task<Guid> ExecuteAsync(Guid idService, CreateServiceDto service)
    {
        var existingService = await _serviceRepository.GetServiceByIdAsync(idService) ?? throw new InvalidOperationException("Service not found");
        existingService.UpdateService(service.Name, service.Type, existingService.IsActive);
        _serviceRepository.UpdateService(existingService);
        if (!await _uow.CommitAsync())
            throw new InvalidOperationException("Failed to update the service.");

        return existingService.Id;
    }
}
