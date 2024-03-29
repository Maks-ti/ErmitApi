﻿

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ErmitApi.DAL.Models;

public class Achievement : IBaseEntity<int>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int LocationId { get; set; }
    public int ShowplaceId { get; set; }
    public byte[] PictureData { get; set; }
    public string Extension { get; set; }

    public virtual Location Location {  get; set; }
    public virtual Showplace Showplace { get; set; }
    public virtual ICollection<UserAchievement> UserAchievements { get; set; }
}

public class AchievementSourceConfiguration : IEntityTypeConfiguration<Achievement>
{
    public void Configure(EntityTypeBuilder<Achievement> builder)
    {
        builder.ToTable("achievement");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.Name)
            .HasColumnName("name");

        builder.Property(x => x.Description)
            .HasColumnName("description");

        builder.Property(x => x.LocationId)
            .HasColumnName("location_id");

        builder.Property(x => x.ShowplaceId)
            .HasColumnName("showplace_id");

        builder.Property(x => x.PictureData)
            .HasColumnName("picture_data");

        builder.Property(x => x.Extension)
            .HasColumnName("picture_extension");

        builder.HasOne(x => x.Location)
            .WithMany(location => location.Achievements)
            .HasForeignKey(x => x.LocationId);

        builder.HasOne(x => x.Showplace)
            .WithMany(Showplace => Showplace.Achievements)
            .HasForeignKey(x => x.ShowplaceId);
    }
}
