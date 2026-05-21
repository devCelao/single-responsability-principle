using domain.Repositories;

namespace application.UseCases.Plan;

public class DeletePlanUseCase(IPlanRepository planRepository, IUnitOfWork unitOfWork)
{
    private readonly IPlanRepository _planRepository = planRepository;

    private readonly IUnitOfWork _uow = unitOfWork;

    public async Task ExecuteAsync(Guid idPlan)
    {
        var plan = await _planRepository.GetPlanByIdAsync(idPlan) ?? throw new InvalidOperationException("Plan not found");
        _planRepository.DeletePlanAsync(plan);
        if (!await _uow.CommitAsync())
            throw new InvalidOperationException("Failed to delete the plan.");
    }
}
