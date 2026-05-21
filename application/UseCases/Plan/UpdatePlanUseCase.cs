using application.Dtos;
using domain.Repositories;

namespace application.UseCases.Plan;

public class UpdatePlanUseCase(IPlanRepository planRepository, IUnitOfWork unitOfWork)
{
    private readonly IPlanRepository _planRepository = planRepository;

    private readonly IUnitOfWork _uow = unitOfWork;

    public async Task<Guid> ExecuteAsync(Guid idPlan, CreatePlanDto plan)
    {
        var existingPlan = await _planRepository.GetPlanByIdAsync(idPlan) ?? throw new InvalidOperationException("Plan not found");
        existingPlan.UpdatePlan(plan.Name, plan.Description, plan.Price, plan.IsActive);

        _planRepository.UpdatePlan(existingPlan);
        if (!await _uow.CommitAsync())
            throw new InvalidOperationException("Failed to update the plan.");

        return existingPlan.Id;
    }
}
