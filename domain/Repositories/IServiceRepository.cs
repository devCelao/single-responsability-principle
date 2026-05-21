using domain.Entities;

namespace domain.Repositories;

public interface IServiceRepository
{
	Task<Service?> GetServiceByIdAsync(Guid id);
        Task<IEnumerable<Service>> GetByTypeAsync(string type);
        void AddService(Service service);
        void UpdateService(Service service);
        void DeleteServiceAsync(Service service);
}
