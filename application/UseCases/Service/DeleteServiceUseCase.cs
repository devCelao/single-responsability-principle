using domain.Repositories;

namespace application.UseCases.Service;

public class DeleteServiceUseCase(IServiceRepository serviceRepository, IUnitOfWork unitOfWork)
{
    private readonly IServiceRepository _serviceRepository = serviceRepository;
    private readonly IUnitOfWork _uow = unitOfWork;
    public async Task ExecuteAsync(Guid idService)
    {
        var service = await _serviceRepository.GetServiceByIdAsync(idService) ?? throw new InvalidOperationException("Service not found");
        _serviceRepository.DeleteServiceAsync(service);
        if (!await _uow.CommitAsync())
            throw new InvalidOperationException("Failed to delete the service.");
    }
}