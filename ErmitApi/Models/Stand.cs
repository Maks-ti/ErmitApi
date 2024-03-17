
namespace ErmitApi.Models;


public class StandCreateModel
{
    public string Name { get; set; }
    public int LocationId { get; set; }
}

public class StandUpdateModel : StandCreateModel
{
    public int Id { get; set; }
}

public class Stand
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int LocationId { get; set; }
}
