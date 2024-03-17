

using ErmitApi.BLL;
using ErmitApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErmitApi.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class UserAchievementController : ApiControllerBase
{
    private UserAchievementService UserAchievementService { get; set; }
    public UserAchievementController(IServiceProvider serviceProvider, ILogger<UserAchievementController> logger, UserAchievementService service)
        : base(serviceProvider, logger)
    {
        UserAchievementService = service;
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await UserAchievementService.GetAllAsync();

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
            var result = await UserAchievementService.GetByIdAsync(id);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500);
        }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Create([FromBody] UserAchievementCreateModel model)
    {
        try
        {
            var result = await UserAchievementService.CreateAsync(model);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500);
        }
    }

    [HttpPut("[action]")]
    public async Task<IActionResult> Update([FromBody] UserAchievementUpdateModel model)
    {
        try
        {
            var result = await UserAchievementService.UpdateAsync(model);

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
            await UserAchievementService.DeleteByIdAsync(id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500);
        }
    }


}

