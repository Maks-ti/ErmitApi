

namespace ErmitApi.Models;


public class ShowplaceCreateModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Address { get; set; }
    public int LocationId { get; set; }
    public IFormFile PictureFile {  get; set; }
}

public class ShowplaceUpdateModel : ShowplaceCreateModel
{
    public int Id { get; set; }
}

public class Showplace
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int LocationId { get; set; }
    public string Address { get; set; }
    public byte[] PictureData { get; set; }
    public string Extension { get; set; }
}
