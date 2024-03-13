using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Sevices;

public interface IClientdeviceForLitedbService
{
    bool Exist { get; }

    bool Delete(ObjectId id);
    IEnumerable<ClientDevice> GetAllById(ObjectId id);
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

    public IEnumerable<ClientDevice> GetAllById(ObjectId id) => collection.Find(x => x.Id == id);

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

    public bool Delete(ObjectId id) => collection.Delete(id);
}
