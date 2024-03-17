

using AutoMapper;
using ErmitApi.DAL;
using ErmitApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ErmitApi.BLL;


public class UserAchievementService : ServiceBase<DAL.Models.UserAchievement, int>
{
    public UserAchievementService(IServiceProvider serviceProvider, ILogger<UserAchievementService> logger)
    : base(serviceProvider, logger)
    {
        this.Mapper = new MapperConfiguration(cfg =>
        {
            cfg.AllowNullCollections = true;
            cfg.AllowNullDestinationValues = true;

            #region Base To ViewModel
            cfg.CreateMap<DAL.Models.UserAchievement, UserAchievement>();
            #endregion

            #region ViewModel To Base 
            cfg.CreateMap<UserAchievementCreateModel, DAL.Models.UserAchievement>();
            cfg.CreateMap<UserAchievementUpdateModel, DAL.Models.UserAchievement>();
            #endregion
        }).CreateMapper();
    }

    public async Task<List<UserAchievement>> GetAllAsync()
    {
        var baseResult = (await base.GetAllAsync()).ToList();

        return Mapper.Map<List<UserAchievement>>(baseResult);
    }

    public async Task<UserAchievement> GetByIdAsync(int id)
    {
        var baseResult = await base.GetByIdAsync(id);

        return Mapper.Map<UserAchievement>(baseResult);
    }

    public async Task<UserAchievement> CreateAsync(UserAchievementCreateModel model)
    {
        await Validate(model);

        var achievement = Mapper.Map<DAL.Models.UserAchievement>(model);

        var baseResult = await base.AddAsync(achievement);

        return Mapper.Map<UserAchievement>(baseResult);
    }

    public async Task<UserAchievement> UpdateAsync(UserAchievementUpdateModel model)
    {
        await Validate(model);

        var achievement = Mapper.Map<DAL.Models.UserAchievement>(model);

        var baseResult = await base.UpdateAsync(achievement);

        return Mapper.Map<UserAchievement>(baseResult);
    }

    public async Task DeleteAsync(int id)
    {
        await base.DeleteByIdAsync(id);
    }

    private async Task Validate(UserAchievementCreateModel model)
    {
        await using var context = this.GetContext<DataBaseContext>();

        var achievement = await context.Achievements.FirstOrDefaultAsync(ach => ach.Id == model.AchievementId);
        if (achievement == null)
        {
            throw new ArgumentException($"Achievement  id = {model.AchievementId} does not exists");
        }

        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == model.UserId);
        if (user == null)
        {
            throw new ArgumentException($"User id = {model.UserId} does not exists");
        }

        return;
    }
}

