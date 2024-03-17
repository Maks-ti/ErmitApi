

using ErmitApi.BLL;
using ErmitApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErmitApi.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class ShowplaceController : ApiControllerBase
{
    private ShowplaceService ShowplaceService { get; set; }
    public ShowplaceController(IServiceProvider serviceProvider, ILogger<ShowplaceController> logger, ShowplaceService service)
        : base(serviceProvider, logger)
    {
        ShowplaceService = service;
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await ShowplaceService.GetAllAsync();

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
            var result = await ShowplaceService.GetByIdAsync(id);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500);
        }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Create([FromForm] ShowplaceCreateModel model)
    {
        try
        {
            var result = await ShowplaceService.CreateAsync(model);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500);
        }
    }

    [HttpPut("[action]")]
    public async Task<IActionResult> Update([FromForm] ShowplaceUpdateModel model)
    {
        try
        {
            var result = await ShowplaceService.UpdateAsync(model);

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
            await ShowplaceService.DeleteByIdAsync(id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500);
        }
    }


}
