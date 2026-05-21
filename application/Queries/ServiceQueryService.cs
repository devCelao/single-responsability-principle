using application.Dtos;
using domain.Entities;
using domain.Repositories;

namespace application.Queries;

public class ServiceQueryService(IServiceRepository serviceRepository)
{
    private readonly IServiceRepository _serviceRepository = serviceRepository;

    public async Task<ServiceDto?> GetByIdAsync(Guid id)
    {
        var service = await _serviceRepository.GetServiceByIdAsync(id);
        return MapToDto(service);
    }

    public async Task<IEnumerable<ServiceDto>> GetByTypeAsync(string type)
    {
        var services = await _serviceRepository.GetByTypeAsync(type);
        return services?.Select(MapToDto).Where(dto => dto != null).Cast<ServiceDto>() ?? [];
    }

    private static ServiceDto? MapToDto(Service? service) => service == null ? null : new ServiceDto
    {
        Id = service.Id,
        Name = service.Name,
        Type = service.Type,
    };
}
