using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Sevices;

public interface IClientdeviceForLitedbService
{
    bool Exist { get; }

    bool Delete(string id);
    IEnumerable<ClientDevice> GetAllById(string id);
    string Insert(ClientDevice clientDevice);
    bool Update(ClientDevice clientDevice);
}

public class ClientdeviceForLitedbService : IClientdeviceForLitedbService
{
    readonly ILiteCollection<ClientDevice> collection;

    public ClientdeviceForLitedbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("ClientDevice")
        };

        LiteDatabase db = new(cnx);
        collection = db.GetCollection<ClientDevice>();
    }

    public bool Exist => collection.Count() > 0;

    public IEnumerable<ClientDevice> GetAllById(string id) => collection.Find(x => x.Id == id);

    public string Insert(ClientDevice clientDevice)
    {
        try
        {
            return collection.Insert(clientDevice);
        }
        catch (Exception)
        {

            return string.Empty;
        }
    }

    public bool Update(ClientDevice clientDevice) => collection.Update(clientDevice);

    public bool Delete(string id) => collection.Delete(id);
}
