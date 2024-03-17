

namespace ErmitApi.Models;

public class UserAchievementCreateModel
{
    public Guid UserId { get; set; }
    public int AchievementId { get; set; }
}

public class UserAchievementUpdateModel : UserAchievementCreateModel
{
    public int Id { get; set; }
}

public class UserAchievement
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public int AchievementId { get; set; }
}
