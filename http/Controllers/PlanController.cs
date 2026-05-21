using application.Dtos;
using application.Queries;
using application.UseCases.Plan;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace http.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlanController(IValidator<CreatePlanDto> validator
                          , CreatePlanUseCase creaPlaUseCase
                          , UpdatePlanUseCase updatePlanUseCase
                          , DeletePlanUseCase deletePlanUseCase
                          , PlanQueryService planQuery) : ControllerBase
{
    private readonly IValidator<CreatePlanDto> _validator = validator;
    private readonly CreatePlanUseCase _createPlanUseCase = creaPlaUseCase;
    private readonly UpdatePlanUseCase _updatePlanUseCase = updatePlanUseCase;
    private readonly DeletePlanUseCase _deletePlanUseCase = deletePlanUseCase;
    private readonly PlanQueryService _planQueryService = planQuery;
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePlanDto newPlan)
    {
        var validationResult = await _validator.ValidateAsync(newPlan);
        if (!validationResult.IsValid)  return BadRequest(validationResult.Errors);

        try
        {
            var planId = await _createPlanUseCase.ExecuteAsync(newPlan);

            return CreatedAtAction(nameof(Create), new { id = planId }, new { id = planId, message = "Plan successfully created!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var plan = await _planQueryService.GetByIdAsync(id);
        if (plan is null) return NotFound();
        return Ok(plan);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var plans = await _planQueryService.GetAllAsync();
        return Ok(plans);
    }

    [HttpPut("{planId}")]
    public async Task<IActionResult> UpdatePlan(Guid planId, CreatePlanDto planDto)
    {
        var validationResult = await _validator.ValidateAsync(planDto);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
        try
        {
            var updatedPlanId = await _updatePlanUseCase.ExecuteAsync(planId, planDto);
            return Ok(new { id = updatedPlanId, message = "Plan successfully updated!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpDelete("{planId}")]
    public async Task<IActionResult> DeletePlan(Guid planId)
    {
        try
        {
            await _deletePlanUseCase.ExecuteAsync(planId);
            return Ok(new { id = planId, message = "Plan successfully deleted!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}
