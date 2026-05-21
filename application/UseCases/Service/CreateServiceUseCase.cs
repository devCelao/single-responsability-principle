using application.Dtos;
using domain.Repositories;

namespace application.UseCases.Service;

public class CreateServiceUseCase(IServiceRepository serviceRepository, IUnitOfWork unitOfWork)
{
    private readonly IServiceRepository _serviceRepository = serviceRepository;
    private readonly IUnitOfWork _uow = unitOfWork;
    public async Task<Guid> ExecuteAsync(CreateServiceDto createService)
    {
        var service = new domain.Entities.Service(createService.Name, createService.Type);
        _serviceRepository.AddService(service);
        if (!await _uow.CommitAsync())
            throw new InvalidOperationException("Failed to create the service.");
        return service.Id;
    }
}