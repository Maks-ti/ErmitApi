

using ErmitApi.BLL;
using ErmitApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErmitApi.Controllers;


[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class StandController : ApiControllerBase
{
    private StandService StandService { get; set; }
    public StandController(IServiceProvider serviceProvider, ILogger<StandController> logger, StandService service)
        : base(serviceProvider, logger)
    {
        StandService = service;
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await StandService.GetAllAsync();

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
            var result = await StandService.GetByIdAsync(id);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500);
        }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Create([FromBody] StandCreateModel model)
    {
        try
        {
            var result = await StandService.CreateAsync(model);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500);
        }
    }

    [HttpPut("[action]")]
    public async Task<IActionResult> Update([FromBody] StandUpdateModel model)
    {
        try
        {
            var result = await StandService.UpdateAsync(model);

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
            await StandService.DeleteByIdAsync(id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500);
        }
    }


}

