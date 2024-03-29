﻿
using ErmitApi.Models;
using ErmitApi.BLL;
using Microsoft.AspNetCore.Mvc;

namespace ErmitApi.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AchievementController : ApiControllerBase
{
    private AchievementService AchievementService { get; set; }
    public AchievementController(IServiceProvider serviceProvider, ILogger<AchievementController> logger, AchievementService service)
        : base(serviceProvider, logger)
    {
        AchievementService = service;
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await AchievementService.GetAllAsync();

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
            var result = await AchievementService.GetByIdAsync(id);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500);
        }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Create([FromForm] AchievementCreateModel model)
    {
        try
        {
            var result = await AchievementService.CreateAsync(model);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500);
        }
    }

    [HttpPut("[action]")]
    public async Task<IActionResult> Update([FromForm] AchievementUpdateModel model)
    {
        try
        {
            var result = await AchievementService.UpdateAsync(model);

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
            await AchievementService.DeleteByIdAsync(id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500);
        }
    }


}
