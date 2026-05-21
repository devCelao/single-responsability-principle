using domain.Entities;

namespace domain.Repositories;

public interface IPlanRepository
{
    Task<Plan?> GetPlanByIdAsync(Guid id);
    Task<IEnumerable<Plan>> GetAllPlansAsync();
    void AddPlan(Plan plan);
    void UpdatePlan(Plan plan);
    void DeletePlanAsync(Plan plan);
}
