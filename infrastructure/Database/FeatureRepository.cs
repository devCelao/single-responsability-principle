using domain.Entities;
using domain.Repositories;
using infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.Database;

public class FeatureRepository(AppDbContext context) : IFeatureRepository
{
    private readonly AppDbContext _context = context;
    public void AddFeature(Feature feature)
        => _context.Features.Add(feature);

    public void DeleteFeature(Feature feature)
        => _context.Features.Remove(feature);
    public async Task<Feature?> GetFeatureByIdAsync(Guid id)
        => await _context.Features.FindAsync(id);
    public async Task<Feature?> GetFeatureByNameAsync(string name)
        => await _context.Features.FirstOrDefaultAsync(f => f.Name == name);
    public void UpdateFeature(Feature feature)
        => _context.Features.Update(feature);
}
