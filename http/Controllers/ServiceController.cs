using application.Dtos;
using application.Queries;
using application.UseCases.Service;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace http.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ServiceController(IValidator<CreateServiceDto> validator
                             , CreateServiceUseCase createServiceUseCase
                             , UpdateServiceUseCase updateServiceUseCase
                             , DeleteServiceUseCase deleteServiceUseCase
                             , ServiceQueryService serviceQuery) : ControllerBase
{
    private readonly IValidator<CreateServiceDto> _validator = validator;
    private readonly CreateServiceUseCase _createServiceUseCase = createServiceUseCase; 
    private readonly UpdateServiceUseCase _updateServiceUseCase = updateServiceUseCase;
    private readonly DeleteServiceUseCase _deleteServiceUseCase = deleteServiceUseCase;
    private readonly ServiceQueryService _serviceQueryService = serviceQuery;
   
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateServiceDto newService)
    {
        var validatonResult = await _validator.ValidateAsync(newService);
        if(!validatonResult.IsValid) return BadRequest(new { errors = validatonResult.Errors.Select(e => e.ErrorMessage) });
        try
        {
            var serviceId = await _createServiceUseCase.ExecuteAsync(newService);

            return CreatedAtAction(nameof(Create), new {id = serviceId , message = "Service created successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var service = await _serviceQueryService.GetByIdAsync(id);
        if (service is null) return NotFound();
        return Ok(service);
    }

    [HttpGet("type/{type}")]
    public async Task<IActionResult> GetByType(string type)
    {
        var services = await _serviceQueryService.GetByTypeAsync(type);
        return Ok(services);
    }

    [HttpPut("{serviceId}")]
    public async Task<IActionResult> UpdateService(Guid serviceId, CreateServiceDto serviceDto)
    {
        var validatonResult = await _validator.ValidateAsync(serviceDto);
        if (!validatonResult.IsValid) return BadRequest(new { errors = validatonResult.Errors.Select(e => e.ErrorMessage) });
        try
        {
            var updatedServiceId = await _updateServiceUseCase.ExecuteAsync(serviceId, serviceDto);
            return Ok(new { id = serviceId, message = "Service updated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpDelete("{serviceId}")]
    public async Task<IActionResult> DeleteService(Guid serviceId)
    {
        try
        {
            await _deleteServiceUseCase.ExecuteAsync(serviceId);
            return Ok(new { id = serviceId, message = "Service deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}
