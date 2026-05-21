using application.Dtos;
using domain.Entities;
using domain.Repositories;

namespace application.Queries;

public class PlanQueryService(IPlanRepository planRepository)
{
    private readonly IPlanRepository _planRepository = planRepository;
    public async Task<PlanDto?> GetByIdAsync(Guid id)
    {
        var plan = await _planRepository.GetPlanByIdAsync(id) ?? throw new KeyNotFoundException($"Plan with id {id} not found.");
        return MapToDto(plan);
    }
    public async Task<IEnumerable<PlanDto>> GetAllAsync()
    {
        var plans = await _planRepository.GetAllPlansAsync();
        return plans.Select(MapToDto);
    }
    private static PlanDto MapToDto(Plan plan) => new()
    {
        Id = plan.Id,
        Name = plan.Name,
        Description = plan.Description,
        Price = plan.Price,
        IsActive = plan.IsActive
    };
}