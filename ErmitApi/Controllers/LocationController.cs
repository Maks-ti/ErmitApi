
using ErmitApi.Models;
using ErmitApi.BLL;
using Microsoft.AspNetCore.Mvc;


namespace ErmitApi.Controllers;


[ApiController]
[Route("api/[controller]")]
public class LocationController : ApiControllerBase
{
    private LocationService LocationService { get; set; }
    public LocationController(IServiceProvider serviceProvider, ILogger<LocationController> logger, LocationService service)
        : base(serviceProvider, logger)
    {
        LocationService = service;
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await LocationService.GetAllAsync();

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500);
        }
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> Get([FromQuery] int id)
    {
        try
        {
            var result = await LocationService.GetByIdAsync(id);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500);
        }
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetByStandId ([FromQuery] int id)
    {
        try
        {
            var result = await LocationService.GetByStandIdAsync(id);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500);
        }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Create([FromForm] LocationCreateModel model)
    {
        try
        {
            var result = await LocationService.CreateAsync(model);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500);
        }
    }

    [HttpPut("[action]")]
    public async Task<IActionResult> Update([FromForm] LocationUpdateModel model)
    {
        try
        {
            var result = await LocationService.UpdateAsync(model);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500);
        }
    }

    [HttpDelete("[action]")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await LocationService.DeleteByIdAsync(id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500);
        }
    }


}
