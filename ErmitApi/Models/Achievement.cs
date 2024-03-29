﻿
namespace ErmitApi.Models;

public class AchievementCreateModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int? LocationId { get; set; }
    public int? ShowplaceId { get; set; }
    public IFormFile PictureFile { get; set; }
}

public class AchievementUpdateModel : AchievementCreateModel
{
    public int Id { get; set; }
}

public class Achievement
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int LocationId { get; set; }
    public int ShowplaceId { get; set; }
    public byte[] PictureData { get; set; }
    public string Extension { get; set; }
}
