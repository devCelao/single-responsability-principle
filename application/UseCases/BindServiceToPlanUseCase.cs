using domain.Repositories;

namespace application.UseCases;

public class BindServiceToPlanUseCase(IPlanRepository planRepository, IServiceRepository serviceRepository, IUnitOfWork unitOfWork)
{
    private readonly IPlanRepository _planRepository = planRepository;
    private readonly IServiceRepository _serviceRepository = serviceRepository;
    private readonly IUnitOfWork _uow = unitOfWork;

    public async Task ExecuteAsync(Guid planId, Guid serviceId)
    {
        var plan = await _planRepository.GetPlanByIdAsync(planId) ?? throw new KeyNotFoundException("Plan not found.");
        var service = await _serviceRepository.GetServiceByIdAsync(serviceId) ?? throw new KeyNotFoundException("Service not found.");
        if (plan.Services.Any(s => s.Id == serviceId))
            throw new InvalidOperationException("The service is already linked to the plan.");
        plan.Services.Add(service);

        _planRepository.UpdatePlan(plan);

        if(!await _uow.CommitAsync())
            throw new Exception("Failed to bind service to plan.");
    }
}
