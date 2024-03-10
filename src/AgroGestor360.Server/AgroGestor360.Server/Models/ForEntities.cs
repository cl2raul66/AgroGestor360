using LiteDB;
using vCardLib.Models;

namespace AgroGestor360.Server.Models;

public class ClientDevice
{
    public ObjectId? Id { get; set; }
    public DeviceInfo? Device { get; set; }
}

public class DeviceInfo
{
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public string? SO { get; set; }
}

public class User
{
    public ObjectId? Id { get; set; }
    public vCard? Contact { get; set; }
    public bool IsAuthorized { get; set; }
    public List<string>? GroupsId { get; set; }
}

public class ClientClass
{
    public ObjectId? Id { get; set; }
    public string? Title { get; set; }
    public double? PercentageValue { get; set; }
}

public class UserGroup
{
    public string? Id { get; set; }
    public string? Name { get; set; }
}

public class Merchandise
{
    public ObjectId? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class Inventory
{
    public ObjectId? Id { get; set; }
    public ObjectId? MerchandiseId { get; set; }
    public double Quantity { get; set; }
}

public class Presentation
{
    public string Measure { get; set; }
    public string Unit { get; set; }
    public double Value { get; set; }
}

public class Article
{
    public ObjectId? Id { get; set; }
    public string? Name { get; set; }
    public double Price { get; set; }
    public Presentation? Packaging { get; set; }
    public string? Category { get; set; }
}

public class Product
{
    public ObjectId? Id { get; set; }
    public Article? Article { get; set; }
    public double Quantity { get; set; }
}

public class ProductOffering
{
    public ObjectId? Id { get; set; }
    public ObjectId? ProductId { get; set; }
    public double Quantity { get; set; }
    public double BonusAmount { get; set; }
}

public class Token
{
    public string? Id { get; set; }
}
