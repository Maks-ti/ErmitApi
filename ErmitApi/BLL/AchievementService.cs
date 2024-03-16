

using AutoMapper;
using ErmitApi.Models;

namespace ErmitApi.BLL;

public class AchievementService : ServiceBase<DAL.Models.Achievement, int, Achievement, AchievementCreateModel>
{
    public AchievementService(IServiceProvider serviceProvider, ILogger<AchievementService> logger)
        : base(serviceProvider, logger) { }


}
 