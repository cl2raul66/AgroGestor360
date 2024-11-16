using AgroGestor360Server.Models;
using AgroGestor360Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360Server.Services;

public interface IClientdeviceForLitedbService : IDisposable
{
    bool Exist { get; }

    bool Delete(ObjectId id);
    IEnumerable<ClientDevice> GetAllById(ObjectId id);
    string Insert(ClientDevice clientDevice);
    bool Update(ClientDevice clientDevice);
}

public class ClientdeviceForLitedbService : IClientdeviceForLitedbService
{
    readonly LiteDatabase db;
    readonly ILiteCollection<ClientDevice> collection;

    public ClientdeviceForLitedbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("ClientDevice")
        };

        db = new(cnx);
        collection = db.GetCollection<ClientDevice>();
    }

    public void BeginTrans() => db.BeginTrans();

    public void Commit() => db.Commit();

    public void Rollback() => db.Rollback();

    public void Dispose() => db.Dispose();

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
