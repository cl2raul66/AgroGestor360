using vCardLib.Models;

namespace AgroGestor360.Server.Models;

public class ClientDevice
{
    public string? Id { get; set; }
    public object? Device { get; set; }
}

public class User
{
    public string? Id { get; set; }
    public vCard? Contact { get; set; }
}

public class Token
{
    public string? Id { get; set; }
}
