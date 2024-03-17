
namespace ErmitApi.Models;


public class LocationCreateModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public IFormFile PictureFile { get; set; }
}

public class LocationUpdateModel : LocationCreateModel
{
    public int Id { get; set; }
}

public class Location
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public byte[] PictureData { get; set; }
    public string Extension { get; set; }

    public IEnumerable<Showplace> Showplaces { get; set; }
}

