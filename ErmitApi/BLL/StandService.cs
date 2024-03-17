

using AutoMapper;
using ErmitApi.DAL;
using ErmitApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ErmitApi.BLL;


public class StandService : ServiceBase<DAL.Models.Stand, int>
{
    public StandService(IServiceProvider serviceProvider, ILogger<StandService> logger)
    : base(serviceProvider, logger)
    {
        this.Mapper = new MapperConfiguration(cfg =>
        {
            cfg.AllowNullCollections = true;
            cfg.AllowNullDestinationValues = true;

            #region Base To ViewModel
            cfg.CreateMap<DAL.Models.Stand, Stand>();
            #endregion

            #region ViewModel To Base 
            cfg.CreateMap<StandCreateModel, DAL.Models.Stand>();
            cfg.CreateMap<StandUpdateModel, DAL.Models.Stand>();
            #endregion
        }).CreateMapper();
    }

    public async Task<List<Stand>> GetAllAsync()
    {
        var baseResult = (await base.GetAllAsync()).ToList();

        return Mapper.Map<List<Stand>>(baseResult);
    }

    public async Task<Stand> GetByIdAsync(int id)
    {
        var baseResult = await base.GetByIdAsync(id);

        return Mapper.Map<Stand>(baseResult);
    }

    public async Task<Stand> CreateAsync(StandCreateModel model)
    {
        await Validate(model);

        var achievement = Mapper.Map<DAL.Models.Stand>(model);

        var baseResult = await base.AddAsync(achievement);

        return Mapper.Map<Stand>(baseResult);
    }

    public async Task<Stand> UpdateAsync(StandUpdateModel model)
    {
        await Validate(model);

        var achievement = Mapper.Map<DAL.Models.Stand>(model);

        var baseResult = await base.UpdateAsync(achievement);

        return Mapper.Map<Stand>(baseResult);
    }

    public async Task DeleteAsync(int id)
    {
        await base.DeleteByIdAsync(id);
    }

    private async Task Validate(StandCreateModel model)
    {
        await using var context = this.GetContext<DataBaseContext>();

        var location = await context.Locations.FirstOrDefaultAsync(l => l.Id == model.LocationId);
        if (location == null)
        {
            throw new ArgumentException($"Location id = {model.LocationId} does not exists");
        }

        return;
    }
}

