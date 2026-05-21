using domain.Entities;
using domain.Repositories;
using infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.Database;

public class ServiceRepository(AppDbContext context) : IServiceRepository
{
    private readonly AppDbContext _context = context;
    public void AddService(Service service)
        => _context.Services.Add(service);

    public void DeleteServiceAsync(Service service)
        => _context.Services.Remove(service);

    public async Task<IEnumerable<Service>> GetByTypeAsync(string type)
        => await _context.Services.Where(s => s.Type == type).ToListAsync();

    public async Task<Service?> GetServiceByIdAsync(Guid id)
        => await _context.Services.FindAsync(id);

    public void UpdateService(Service service)
        => _context.Services.Update(service);
}
