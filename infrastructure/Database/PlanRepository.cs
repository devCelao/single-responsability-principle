using domain.Entities;
using domain.Repositories;
using infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.Database;

public class PlanRepository(AppDbContext context) : IPlanRepository
{
    private readonly AppDbContext _context = context;
    public void AddPlan(Plan plan)
        => _context.Plans.Add(plan);

    public void DeletePlanAsync(Plan plan)
        => _context.Plans.Remove(plan);

    public async Task<IEnumerable<Plan>> GetAllPlansAsync()
        => await _context.Plans.ToListAsync();

    public async Task<Plan?> GetPlanByIdAsync(Guid id)
        => await _context.Plans.FindAsync(id);

    public void UpdatePlan(Plan plan)
        => _context.Plans.Update(plan);
}
