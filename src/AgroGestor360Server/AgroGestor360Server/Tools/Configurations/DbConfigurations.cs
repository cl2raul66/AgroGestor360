using AgroGestor360Server.Tools.Helpers;
using LiteDB;
using vCardLib.Models;

namespace AgroGestor360Server.Tools.Configurations;

public class BanksDbConfig
{
    public ILiteDatabase Bd { get; }

    public BanksDbConfig()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Banks")
        };

        Bd = new LiteDatabase(cnx);
    }
}

public class ContactsDbConfig
{
    public ILiteDatabase Bd { get; }

    public ContactsDbConfig()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Contacts")
        };

        var mapper = new BsonMapper();
        mapper.Entity<vCard>().Id(vcard => vcard.Uid);

        Bd = new LiteDatabase(cnx, mapper);
    }
}

public class MerchandiseDbConfig
{
    public ILiteDatabase Bd { get; }

    public MerchandiseDbConfig()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Merchandise")
        };

        Bd = new LiteDatabase(cnx);
    }
}

public class ProductsDbConfig
{
    public ILiteDatabase Bd { get; }

    public ProductsDbConfig()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Products")
        };

        Bd = new LiteDatabase(cnx);
    }
}
