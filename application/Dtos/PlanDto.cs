namespace application.Dtos;

public class CreatePlanDto
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}
public class PlanDto : CreatePlanDto
{
    public Guid Id { get; set; }
}