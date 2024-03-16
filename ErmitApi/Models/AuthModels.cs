

namespace ErmitApi.Models;


public class RegistrationModel
{
    public string Login { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string RepeatPassword { get; set; }
    public bool IsAdmin { get; set; }
}

public class LoginModel
{
    public string Login { get; set; }
    public string Password { get; set; }
}
