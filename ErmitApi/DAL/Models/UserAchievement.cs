
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ErmitApi.DAL.Models;

public class UserAchievement : IBaseEntity<int>
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public int AchievementId { get; set;}

    public virtual User User { get; set; }
    public virtual Achievement Achievement { get; set; }
}

public class UserAchievementSourceConfiguration : IEntityTypeConfiguration<UserAchievement>
{
    public void Configure(EntityTypeBuilder<UserAchievement> builder)
    {
        builder.ToTable("user_achievement");
        builder.HasKey(e => e.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id");

        builder.Property(x => x.AchievementId)
            .HasColumnName("achievement_id");

        builder.HasOne(ua => ua.User)
            .WithMany(user => user.UserAchivements)
            .HasForeignKey(ua => ua.UserId);

        builder.HasOne(ua => ua.Achievement)
            .WithMany(achive => achive.UserAchievements)
            .HasForeignKey(ua => ua.AchievementId);
    }
}
