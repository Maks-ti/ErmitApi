

using AutoMapper;
using ErmitApi.DAL;
using ErmitApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ErmitApi.BLL;

public class AchievementService : ServiceBase<DAL.Models.Achievement, int>
{
    public AchievementService(IServiceProvider serviceProvider, ILogger<AchievementService> logger)
        : base(serviceProvider, logger) 
    {
        this.Mapper = new MapperConfiguration(cfg =>
            {
                cfg.AllowNullCollections = true;
                cfg.AllowNullDestinationValues = true;

                #region Base To ViewModel
                cfg.CreateMap<DAL.Models.Achievement, Achievement>();
                #endregion

                #region ViewModel To Base 
                cfg.CreateMap<AchievementCreateModel, DAL.Models.Achievement>()
                    .ForMember(dest => dest.PictureData, opt => opt.MapFrom(src => ConvertToBytes(src.PictureFile)))
                    .ForMember(dest => dest.Extension, opt => opt.MapFrom(src => GetFileExtension(src.PictureFile)));

                cfg.CreateMap<AchievementUpdateModel, DAL.Models.Achievement>()
                    .ForMember(dest => dest.PictureData, opt => opt.MapFrom(src => ConvertToBytes(src.PictureFile)))
                    .ForMember(dest => dest.Extension, opt => opt.MapFrom(src => GetFileExtension(src.PictureFile)));
                #endregion
            }).CreateMapper();
    }

    public async Task<List<Achievement>> GetAllAsync()
    {
        var baseResult = (await base.GetAllAsync()).ToList();

        return Mapper.Map<List<Achievement>>(baseResult);
    }

    public async Task<Achievement> GetByIdAsync(int id)
    {
        var baseResult = await base.GetByIdAsync(id);

        return Mapper.Map<Achievement>(baseResult);
    }

    public async Task<Achievement> CreateAsync(AchievementCreateModel model)
    {
        await Validate(model);

        var achievement = Mapper.Map<DAL.Models.Achievement>(model);

        var baseResult = await base.AddAsync(achievement);

        return Mapper.Map<Achievement>(baseResult);
    }

    public async Task<Achievement> UpdateAsync(AchievementUpdateModel model)
    {
        await Validate(model);

        var achievement = Mapper.Map<DAL.Models.Achievement>(model);

        var baseResult = await base.UpdateAsync(achievement);

        return Mapper.Map<Achievement>(baseResult);
    }
    
    public async Task DeleteAsync(int id)
    {
        await base.DeleteByIdAsync(id);
    }

    private async Task Validate(AchievementCreateModel model)
    {
        await using var context = this.GetContext<DataBaseContext>();

        var location = await context.Locations.FirstOrDefaultAsync(l => l.Id == model.LocationId);
        if (location == null)
        {
            throw new ArgumentException($"Location id = {model.LocationId} does not exists");
        }

        var showplace = await context.Showplaces.FirstOrDefaultAsync(s => s.Id == model.ShowplaceId);
        if (showplace == null)
        {
            throw new ArgumentException($"Showplaces id = {model.ShowplaceId} does not exists");
        }
        
        return;
    }
}
 