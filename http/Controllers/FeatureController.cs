using application.Dtos;
using application.Queries;
using application.UseCases.Feature;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace http.Controllers;
[ApiController]
[Route("api/[controller]")]
public class FeatureController(IValidator<CreateFeatureDto> validator
                             , CreateFeatureUseCase createFeatureUseCase
                             , UpdateFeatureUseCase updateFeatureUseCase
                             , DeleteFeatureUseCase deleteFeatureUseCase
                             , FeatureQueryService featureQuery) : ControllerBase
{
    private readonly IValidator<CreateFeatureDto> _validator = validator;
    private readonly CreateFeatureUseCase _createFeatureUseCase = createFeatureUseCase;
    private readonly UpdateFeatureUseCase _updateFeatureUseCase = updateFeatureUseCase;
    private readonly DeleteFeatureUseCase _deleteFeatureUseCase = deleteFeatureUseCase;
    private readonly FeatureQueryService _featureQueryService = featureQuery;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFeatureDto newFeature)
    {
        var validationResult = await _validator.ValidateAsync(newFeature);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
        try
        {
            var featureId = await _createFeatureUseCase.ExecuteAsync(newFeature);
            return CreatedAtAction(nameof(Create), new { id = featureId }, new { id = featureId, message = "Feature successfully created!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var feature = await _featureQueryService.GetByIdAsync(id);
            if (feature == null) return NotFound(new { message = "Feature not found!" });
            return Ok(feature);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        try
        {
            var feature = await _featureQueryService.GetFeatureByNameAsync(name);
            if (feature == null) return NotFound(new { message = "Feature not found!" });
            return Ok(feature);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPut("{featureId}")]
    public async Task<IActionResult> UpdateFeature(Guid featureId, CreateFeatureDto featureDto)
    {
        var validationResult = await _validator.ValidateAsync(featureDto);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
        try
        {
            await _updateFeatureUseCase.ExecuteAsync(featureId, featureDto);
            return Ok(new { message = "Feature successfully updated!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpDelete("{featureId}")]
    public async Task<IActionResult> DeleteFeature(Guid featureId)
    {
        try
        {
            await _deleteFeatureUseCase.ExecuteAsync(featureId);
            return Ok(new { message = "Feature successfully deleted!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}
