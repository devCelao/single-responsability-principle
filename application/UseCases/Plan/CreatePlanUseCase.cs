using application.Dtos;
using domain.Repositories;

namespace application.UseCases.Plan;

public class CreatePlanUseCase(IPlanRepository planRepository, IUnitOfWork unitOfWork)
{
    private readonly IPlanRepository _planRepository = planRepository;

    private readonly IUnitOfWork _uow = unitOfWork;
    public async Task<Guid> ExecuteAsync(CreatePlanDto createPlan)
    {
        var plan = new domain.Entities.Plan(createPlan.Name, createPlan.Description, createPlan.Price);

        if(createPlan.IsActive)
            plan.Activate();

        _planRepository.AddPlan(plan);

        if(!await _uow.CommitAsync())
            throw new InvalidOperationException("Failed to create the plan.");

        return plan.Id;
    }
}
