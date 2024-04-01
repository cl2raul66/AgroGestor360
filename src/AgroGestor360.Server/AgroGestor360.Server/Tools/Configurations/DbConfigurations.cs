using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Tools.Configurations;

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
